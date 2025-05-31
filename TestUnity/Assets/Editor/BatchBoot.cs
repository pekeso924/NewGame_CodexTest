using UnityEditor;
using UnityEngine;
using System.Linq;

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

        EditorApplication.update -= WaitUntilIdle;
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(0);   // ここでバッチ終了
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
