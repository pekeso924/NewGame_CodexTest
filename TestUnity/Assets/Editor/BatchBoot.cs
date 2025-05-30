using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

[InitializeOnLoad]
internal static class BatchBoot
{
    static BatchBoot()
    {
        Debug.Log("[BatchBoot] コンストラクタが呼び出されました");
        Debug.Log($"[BatchBoot] Application.isBatchMode: {Application.isBatchMode}");
        Debug.Log($"[BatchBoot] HasExecuteMethod(): {HasExecuteMethod()}");
        
        if (!Application.isBatchMode || HasExecuteMethod()) 
        {
            Debug.Log("[BatchBoot] バッチモードではないか、executeMethodが指定されているため、処理をスキップします");
            return;
        }
        
        Debug.Log("[BatchBoot] DelayCallでRefreshAndWaitを登録します");
        EditorApplication.delayCall += RefreshAndWait;
    }

    static void RefreshAndWait()
    {
        Debug.Log("[BatchBoot] RefreshAndWaitが開始されました");
        Debug.Log("[BatchBoot] AssetDatabase.Refreshを実行します");
        
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);

        Debug.Log("[BatchBoot] AssetDatabase.Refreshが完了しました");
        Debug.Log("[BatchBoot] EditorApplication.updateにWaitUntilIdleを登録します");
        
        // フレーム毎に監視
        EditorApplication.update += WaitUntilIdle;
    }

    static void WaitUntilIdle()
    {
        Debug.Log($"[BatchBoot] WaitUntilIdle - isCompiling: {EditorApplication.isCompiling}, isUpdating: {EditorApplication.isUpdating}");
        
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            Debug.Log("[BatchBoot] まだコンパイル中または更新中のため、待機を継続します");
            return;
        }

        Debug.Log("[BatchBoot] アイドル状態になりました。終了処理を開始します");
        EditorApplication.update -= WaitUntilIdle;
        
        Debug.Log("[BatchBoot] AssetDatabase.SaveAssetsを実行します");
        AssetDatabase.SaveAssets();
        
        Debug.Log("[BatchBoot] EditorApplication.Exit(0)でUnityエディタを終了します");
        EditorApplication.Exit(0);
    }

    static bool HasExecuteMethod()
    {
        var hasExecuteMethod = System.Environment.GetCommandLineArgs().Any(a => a == "-executeMethod");
        Debug.Log($"[BatchBoot] HasExecuteMethod結果: {hasExecuteMethod}");
        return hasExecuteMethod;
    }
}

