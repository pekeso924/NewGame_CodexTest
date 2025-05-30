using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;

[InitializeOnLoad]
internal static class BatchBoot
{
    static BatchBoot()
    {
        LogToConsole("=== BatchBoot: 初期化開始 ===");
        LogToConsole($"BatchBoot: Application.isBatchMode = {Application.isBatchMode}");
        LogToConsole($"BatchBoot: HasExecuteMethod() = {HasExecuteMethod()}");
        
        if (!Application.isBatchMode || HasExecuteMethod()) 
        {
            LogToConsole("BatchBoot: バッチモードではないか、executeMethodが指定されているため処理をスキップします");
            return;
        }
        
        LogToConsole("BatchBoot: delayCallにRefreshAndWaitを登録します");
        EditorApplication.delayCall += RefreshAndWait;
        LogToConsole("=== BatchBoot: 初期化完了 ===");
    }

    static void RefreshAndWait()
    {
        LogToConsole("=== RefreshAndWait: 開始 ===");
        LogToConsole("RefreshAndWait: AssetDatabase.Refreshを実行します...");
        
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);
        
        LogToConsole("RefreshAndWait: AssetDatabase.Refresh完了");
        LogToConsole("RefreshAndWait: WaitUntilIdleをEditorApplication.updateに登録します");
        
        // フレーム毎に監視
        EditorApplication.update += WaitUntilIdle;
        LogToConsole("=== RefreshAndWait: 完了 ===");
    }

    static void WaitUntilIdle()
    {
        LogToConsole($"WaitUntilIdle: チェック中... isCompiling={EditorApplication.isCompiling}, isUpdating={EditorApplication.isUpdating}");
        
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            LogToConsole("WaitUntilIdle: まだコンパイル中またはアップデート中のため待機します");
            return;
        }

        LogToConsole("=== WaitUntilIdle: アイドル状態を検出、終了処理開始 ===");
        LogToConsole("WaitUntilIdle: EditorApplication.updateからWaitUntilIdleを削除します");
        EditorApplication.update -= WaitUntilIdle;
        
        LogToConsole("WaitUntilIdle: AssetDatabase.SaveAssetsを実行します...");
        AssetDatabase.SaveAssets();
        LogToConsole("WaitUntilIdle: AssetDatabase.SaveAssets完了");
        
        LogToConsole("WaitUntilIdle: EditorApplication.Exit(0)でUnityエディターを終了します");
        LogToConsole("=== BatchBoot処理完了 - Unityを終了します ===");
        EditorApplication.Exit(0);
    }

    static bool HasExecuteMethod() =>
        System.Environment.GetCommandLineArgs().Any(a => a == "-executeMethod");
    
    /// <summary>
    /// コンソールとUnityログの両方に出力
    /// </summary>
    static void LogToConsole(string message)
    {
        string timestampedMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
        
        // Unityのログに出力
        Debug.Log(timestampedMessage);
        
        // コンソールに直接出力（バッチモード時に確実に見えるように）
        Console.WriteLine(timestampedMessage);
        
        // 即座にフラッシュしてバッファリングを防ぐ
        Console.Out.Flush();
    }
}

