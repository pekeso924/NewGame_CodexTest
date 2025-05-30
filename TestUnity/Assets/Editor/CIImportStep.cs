// Assets/Editor/CIImportStep.cs
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading;

public static class CIImportStep
{
    public static void ImportAndQuit()
    {
        // 1️⃣ すべてのアセットを同期でインポート
        AssetDatabase.Refresh(
            ImportAssetOptions.ForceUpdate |
            ImportAssetOptions.ForceSynchronousImport);

        // 2️⃣ スクリプト再コンパイルや非同期インポートが終わるまで待機
        while (EditorApplication.isCompiling ||
               CompilationPipeline.isCompiling ||
               EditorApplication.isUpdating)
        {
            Thread.Sleep(100);    // 0.1 秒ごとにポーリング
        }

        // 3️⃣ .meta / Asset 情報を確実にディスクへ
        AssetDatabase.SaveAssets();

        // 4️⃣ 正常終了
        EditorApplication.Exit(0);
    }
}
