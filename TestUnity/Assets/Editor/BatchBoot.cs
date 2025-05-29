// Assets/Editor/BatchBoot.cs
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class BatchBoot
{
    static BatchBoot()
    {
        // CLI で起動された Editor だけを対象にする
        if (Application.isBatchMode && !HasExecuteMethod())
        {
            CIImportStep.ImportAndQuit();
        }
    }

    private static bool HasExecuteMethod()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
            if (args[i] == "-executeMethod") return true;
        return false;
    }
}
