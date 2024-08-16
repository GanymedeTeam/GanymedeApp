using UnityEngine;
using TMPro;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using UnityEngine.Networking;

public class VersionManager : MonoBehaviour
{
    public TMP_Text version;
    private const string UniqueIDKey = "ClientID";
    public GameObject UpdateNotification;

    private void AppPrerequisites()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/guides"))
            Directory.CreateDirectory(Application.persistentDataPath + "/guides");
    }

    void Awake()
    {
        version.text = $"v{Application.version}";
        AppPrerequisites();
        GenerateClientID();
        if (PlayerPrefs.GetString("AppVersion", "None") != Application.version || PlayerPrefs.GetInt("sendVAtNextBoot", 0) == 1)
        {
            PlayerPrefs.SetString("AppVersion", Application.version);
            StartCoroutine(SendUniqueIDToApi());
        }
        StartCoroutine(GetLatestReleaseTag());
    }

    private void GenerateClientID()
    {
        if (!PlayerPrefs.HasKey(UniqueIDKey))
        {
            string uniqueID = System.Guid.NewGuid().ToString();
            string hashedID = HashUniqueID(uniqueID);
            PlayerPrefs.SetString(UniqueIDKey, hashedID);
            PlayerPrefs.Save();
        }
        else
        {
            string uniqueID = PlayerPrefs.GetString(UniqueIDKey);
        }
    }

    private string HashUniqueID(string uniqueID)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueID));
        StringBuilder builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }

    private IEnumerator SendUniqueIDToApi()
    {
        WWWForm form = new WWWForm();
        string uniqueID = PlayerPrefs.GetString(UniqueIDKey);
        form.AddField("uniqueID", uniqueID);
        form.AddField("version", Application.version);
#if UNITY_EDITOR
        form.AddField("os", "UnityEditor");
#elif UNITY_STANDALONE_OSX
        form.AddField("os", "Mac_OS");
#else
        form.AddField("os", "Windows");
#endif
        string lang = PlayerPrefs.GetString("lang", "fr");
        if (lang == "po")
            lang = "pt";
        form.AddField("lang", lang);

        using UnityWebRequest www = UnityWebRequest.Post($"{Constants.ganymedeWebUrl}/api/downloaded", form);
        www.timeout = 2;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            PlayerPrefs.SetInt("sendVAtNextBoot", 1);
            Debug.LogError("Erreur lors de l'envoi des informations: " + www.error);
        }
        else
        {
            PlayerPrefs.SetInt("sendVAtNextBoot", 0);
            Debug.Log("Envoi unique du téléchargement de la nouvelle version avec succès!");
        }
    }

    [Serializable]
    public class GitHubRelease
    {
        public string tag_name;
    }

    private IEnumerator GetLatestReleaseTag()
    {
        string apiUrl = "https://api.github.com/repos/GanymedeTeam/GanymedeApp/releases/latest";
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("User-Agent", "UnityApp");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
        {
            string jsonResponse = request.downloadHandler.text;
            GitHubRelease release = JsonUtility.FromJson<GitHubRelease>(jsonResponse);
            if (release.tag_name != $"v{Application.version}")
            {
                UpdateNotification.SetActive(true);
                Debug.Log($"Latest release tag name: {release.tag_name}, current version is v{Application.version}");
            }
        }
    }
}
