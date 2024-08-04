using UnityEngine;
using System.IO;
using System;

public class RotatingLogHandler : ILogHandler
{
    private ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;
    private StreamWriter logFile;
    private string logFilePath;
    private long maxFileSize;
    private int maxFileCount;

    public RotatingLogHandler(string filePath, long maxFileSize, int maxFileCount)
    {
        this.logFilePath = filePath;
        this.maxFileSize = maxFileSize;
        this.maxFileCount = maxFileCount;
        InitializeLogFile();
    }

    private void InitializeLogFile()
    {
        if (logFile != null)
        {
            logFile.Close();
        }

        RotateLogs();
        logFile = new StreamWriter(logFilePath, true);
    }

    private void RotateLogs()
    {
        if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > maxFileSize)
        {
            logFile.Close();

            for (int i = maxFileCount - 1; i > 0; i--)
            {
                string oldFilePath = $"{logFilePath}.{i}";
                string newFilePath = $"{logFilePath}.{i + 1}";

                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }

                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                }
            }

            File.Move(logFilePath, $"{logFilePath}.1");
        }
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        string message = string.Format(format, args);
        logFile.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {message}");
        logFile.Flush();

        RotateLogs();
        defaultLogHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        logFile.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [Exception] {exception}");
        logFile.Flush();

        RotateLogs();
        try
        {
            defaultLogHandler.LogException(exception, context);
        } catch {}
    }

    public void Close()
    {
        logFile.Close();
    }
}
