// Assets/Editor/LogSplitter.cs（Editor 対象なら Assembly Definition で限定）
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LogSplitter
{
    static readonly string ErrorLogPath =
        Path.Combine("Logs", $"errors_{System.DateTime.Now:yyyyMMdd_HHmmss}.log");

    static LogSplitter()
    {
        // Info のスタックトレースは切ってログ量を抑制
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        // Error/Exception だけを別ファイルへ
        Application.logMessageReceivedThreaded += (cond, trace, type) =>
        {
            if (type == LogType.Error || type == LogType.Exception ||
                type == LogType.Assert)
            {
                File.AppendAllText(ErrorLogPath,
                    $"{type}: {cond}\n{trace}\n");
            }
        };
    }
}
