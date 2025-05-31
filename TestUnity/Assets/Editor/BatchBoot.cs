using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
internal static class BatchBoot
{
    // ──────────────────────────────────────────────────────────────
    //  Static constructor
    // ──────────────────────────────────────────────────────────────
    static BatchBoot()
    {
        // バッチモードでない／-executeMethod 指定あり／-runTests 系指定あり
        // いずれかなら何もしない
        if (!Application.isBatchMode || HasExecuteMethod() || IsRunTests())
            return;

        // アセットリフレッシュを 1 フレーム後に実行
        EditorApplication.delayCall += RefreshAndWait;
    }

    // ──────────────────────────────────────────────────────────────
    //  Asset refresh
    // ──────────────────────────────────────────────────────────────
    static void RefreshAndWait()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);

        // 毎フレームコンパイル／更新完了を監視
        EditorApplication.update += WaitUntilIdle;
    }

    // ──────────────────────────────────────────────────────────────
    //  Wait until Editor is idle
    // ──────────────────────────────────────────────────────────────
    static void WaitUntilIdle()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        // ---- NEW: すべてのシーンを BuildSettings に自動登録 -----------------
        EnsureScenesInBuildSettings();
        // ----------------------------------------------------------------------

        EditorApplication.update -= WaitUntilIdle;
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(0);   // ここでバッチ終了
    }

    // ──────────────────────────────────────────────────────────────
    //  Auto-register scenes with correct GUIDs
    // ──────────────────────────────────────────────────────────────
    static void EnsureScenesInBuildSettings()
    {
        // プロジェクト内のすべてのシーンパスを取得
        var allScenePaths = AssetDatabase.FindAssets("t:Scene")
                                         .Select(AssetDatabase.GUIDToAssetPath)
                                         .Where(p => p.EndsWith(".unity"))
                                         .ToList();

        // 既存の BuildSettings シーンを取得
        var existing = EditorBuildSettings.scenes.ToList();
        var existingPaths = existing.Select(s => s.path).ToHashSet();

        bool updated = false;

        // 未登録のシーンを追加
        foreach (var path in allScenePaths)
        {
            if (!existingPaths.Contains(path))
            {
                existing.Add(new EditorBuildSettingsScene(path, /*enabled*/ true));
                updated = true;
            }
        }

        if (updated)
        {
            EditorBuildSettings.scenes = existing.ToArray();
            Debug.Log($"[BatchBoot] Auto-registered {existing.Count - existingPaths.Count} new scene(s) to BuildSettings.");
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────────────────────
    static bool HasExecuteMethod() =>
        System.Environment.GetCommandLineArgs().Any(a => a == "-executeMethod");

    static bool IsRunTests() =>
        System.Environment.GetCommandLineArgs()
            .Any(a => a == "-runTests" || a == "-runEditorTests");
}
