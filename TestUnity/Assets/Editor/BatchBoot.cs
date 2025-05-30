using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

[InitializeOnLoad]
internal static class BatchBoot
{
    static BatchBoot()
    {
        if (!Application.isBatchMode || HasExecuteMethod()) return;
        EditorApplication.delayCall += RefreshAndWait;
    }

    static void RefreshAndWait()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);

        // フレーム毎に監視
        EditorApplication.update += WaitUntilIdle;
    }

    static void WaitUntilIdle()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        EditorApplication.update -= WaitUntilIdle;
        AssetDatabase.SaveAssets();
        EditorApplication.Exit(0);
    }

    static bool HasExecuteMethod() =>
        System.Environment.GetCommandLineArgs().Any(a => a == "-executeMethod");
}

