// Assets/Editor/CIStartupHook.cs
#if UNITY_2022_3_OR_NEWER          // 旧バージョンでもコンパイルが通るようにガード
using UnityEditor;
using UnityEditor.Build;           // IProjectStartupHook が入っている名前空間
using UnityEngine;

public sealed class CIStartupHook : IProjectStartupHook
{
    public void OnStartup()
    {
        // GUI で Editor を開いたときは何もしない
        if (Application.isBatchMode)
        {
            // すでに別の -executeMethod が指定されている場合は任せる
            if (!HasExecuteMethodArg())
                CIImportStep.ImportAndQuit();
        }
    }

    private bool HasExecuteMethodArg()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
            if (args[i] == "-executeMethod")
                return true;
        return false;
    }
}
#endif
