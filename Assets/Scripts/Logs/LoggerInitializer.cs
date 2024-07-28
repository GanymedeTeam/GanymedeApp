using UnityEngine;
using System.IO;

public class LoggerInitializer : MonoBehaviour
{
    private RotatingLogHandler rotatingLogHandler;

    void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/logs"))
            Directory.CreateDirectory(Application.persistentDataPath + "/logs");
        string logFilePath = Path.Combine(Application.persistentDataPath, "logs/log.txt");
        long maxFileSize = 10 * 1024 * 1024; // Taille maximale de 10 MB par fichier de log
        int maxFileCount = 5; // Conserver les 5 derniers fichiers de log

        rotatingLogHandler = new RotatingLogHandler(logFilePath, maxFileSize, maxFileCount);
        Debug.unityLogger.logHandler = rotatingLogHandler;
    }

    void OnDestroy()
    {
        if (rotatingLogHandler != null)
        {
            rotatingLogHandler.Close();
        }
    }
}
