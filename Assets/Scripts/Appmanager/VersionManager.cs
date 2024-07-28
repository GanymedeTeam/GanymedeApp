using UnityEngine;
using TMPro;
using System.IO;

public class VersionManager : MonoBehaviour
{
    public TMP_Text version;

    private void AppPrerequisites()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/guides"))
            Directory.CreateDirectory(Application.persistentDataPath + "/guides");
        if (!Directory.Exists(Application.persistentDataPath + "/guideSaves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/guideSaves");
    }

    void Awake()
    {
        version.text = $"v{Application.version}";
        AppPrerequisites();
    }
}
