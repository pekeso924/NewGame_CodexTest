// Assets/Editor/CIImportStep.cs
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading;
using System.Diagnostics;

public static class CIImportStep
{
    public static void ImportAndQuit()
    {
        var sw = Stopwatch.StartNew();
        UnityEngine.Debug.Log("[CIImportStep] === ImportAndQuit START ===");

        // 1. 同期で全インポート
        UnityEngine.Debug.Log("[CIImportStep] AssetDatabase.Refresh ...");
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate |
                              ImportAssetOptions.ForceSynchronousImport);
        UnityEngine.Debug.LogFormat("[CIImportStep] Refresh done ({0:0.0}s)", sw.Elapsed.TotalSeconds);

        // 2. 追加コンパイルなどが終わるまで待機
        int loops = 0;
        while (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            if (++loops % 10 == 0)          // 1 秒ごと
            {
                UnityEngine.Debug.LogFormat(
                    "[CIImportStep] Waiting ... {0:0.0}s elapsed",
                    sw.Elapsed.TotalSeconds);
            }
            Thread.Sleep(100);
        }
        UnityEngine.Debug.LogFormat(
            "[CIImportStep] Compile/Update finished ({0:0.0}s)",
            sw.Elapsed.TotalSeconds);

        // 3. ビルド・終了
        AssetDatabase.SaveAssets();
        UnityEngine.Debug.Log("[CIImportStep] Assets saved, exiting Unity");
        EditorApplication.Exit(0);
    }

    // ---- 追加: コンパイルイベントにもフックしておく ----
    static CIImportStep()
    {
        // compilationStarted は Action<object> なので
        // ダミー引数（_）を受け取る形に修正
        CompilationPipeline.compilationStarted  += _ =>
            UnityEngine.Debug.Log("[CIImportStep] Compilation START");
        CompilationPipeline.compilationFinished += _ =>
            UnityEngine.Debug.Log("[CIImportStep] Compilation FINISH");
    }
}
