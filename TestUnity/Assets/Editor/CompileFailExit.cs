#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// CI 用：Editor 起動直後にスクリプトコンパイルエラーを検出したら
/// Editor を exit code 1 で終了させる。
/// </summary>
[InitializeOnLoad]          // Editor 起動時
internal static class CompileFailExit
{
    // 二重実行防止フラグ
    private static bool _checked;

    static CompileFailExit()
    {
        // 起動直後（最初のドメインロード）用
        EditorApplication.delayCall += CheckOnce;
    }

    // スクリプトがリロードされた直後用
    [DidReloadScripts]
    private static void OnScriptsReloaded() => EditorApplication.delayCall += CheckOnce;

    private static void CheckOnce()
    {
        if (_checked) return;          // ガード
        _checked = true;

        // CI 以外（通常起動）では無視
        if (!Application.isBatchMode)  // -batchmode -nographics で true :contentReference[oaicite:3]{index=3}
            return;

        int errorCount = GetCompilerErrorCount();
        if (errorCount > 0)
        {
            Debug.LogError($"🛑  Detected {errorCount} compile error(s); exiting for CI.");
            // 非 0 を返して CI 失敗扱いにする
            EditorApplication.Exit(1); // :contentReference[oaicite:4]{index=4}
        }
    }

    // ───────────────────  internal API 呼び出し（リフレクション） ───────────────────
    private static int GetCompilerErrorCount()
    {
        const BindingFlags Flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        // internal class UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface
        var ecType = typeof(EditorApplication).Assembly
                     .GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
        var method = ecType?.GetMethod("GetCompileErrors", Flags);
        var errors = method?.Invoke(null, null) as Array;
        return errors?.Length ?? 0;
    }
}
#endif
