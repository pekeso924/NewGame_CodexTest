// Assets/Editor/CIImportStep.cs
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading;

public static class CIImportStep
{
    public static void ImportAndQuit()
    {
        // 1. 同期で全インポート
        AssetDatabase.Refresh(
            ImportAssetOptions.ForceUpdate |
            ImportAssetOptions.ForceSynchronousImport);

        // 2. 追加コンパイルなどが終わるまで待機
        while (EditorApplication.isCompiling ||
               EditorApplication.isUpdating)
        {
            Thread.Sleep(100);
        }

        AssetDatabase.SaveAssets();      // 念のため保存
        UnityEngine.Debug.Log("Import & compile finished → Exit");
        EditorApplication.Exit(0);
    }
}
