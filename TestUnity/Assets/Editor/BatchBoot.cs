// Assets/Editor/BatchBoot.cs
using UnityEditor;
using UnityEngine;
using System.Linq;                  // 追加

[InitializeOnLoad]
internal static class BatchBoot
{
    static BatchBoot()
    {
        // ── 起動時にフル情報を吐く ───────────────────────
        var args = System.Environment.GetCommandLineArgs();
        Debug.LogFormat("[BatchBoot] Start - batchMode={0}, args={1}",
                        Application.isBatchMode,
                        string.Join(" ", args));             // ★全部表示

        bool hasExecute = HasExecuteMethod();
        Debug.Log("[BatchBoot] Has -executeMethod? " + hasExecute);

        // CLI で起動された Editor だけを対象にする
        if (Application.isBatchMode && !hasExecute)
        {
            Debug.Log("[BatchBoot] Calling CIImportStep.ImportAndQuit()");
            CIImportStep.ImportAndQuit();
        }
        else
        {
            Debug.Log("[BatchBoot] Import step skipped");
        }
    }

    private static bool HasExecuteMethod()
    {
        return System.Environment.GetCommandLineArgs()
                                 .Any(a => a == "-executeMethod");
    }
}
