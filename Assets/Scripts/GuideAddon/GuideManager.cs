using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class GuideManager : MonoBehaviour
{
    public string guidesCurrentPath;

    private string guidesDraftAndPublicListResponse;
    private string guidesCertifiedListResponse;

    // API call api/guides dont return private guides
    public void onClickGuidesDraftAndPublicList()
    {
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=public", response => {
            guidesDraftAndPublicListResponse = response;
            Debug.Log("Draft and Public Guides Response: " + response);
        }));
    }

    public void onClickGuidesCertifiedList()
    {
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=certified", response => {
            guidesCertifiedListResponse = response;
            Debug.Log("Certified Guides Response: " + response);
        }));
    }

    private IEnumerator GetGuidesList(string url, Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                callback(null);
            }
            else
            {
                callback(webRequest.downloadHandler.text);
            }
        }
    }

    public void onClickDownloadGuide(int id)
    {
        StartCoroutine(DownloadGuide(id));
    }

    private IEnumerator DownloadGuide(int id)
    {
        yield return StartCoroutine(GetGuide(id));
    }

    public void onClickGuide(int id)
    {
        StartCoroutine(OnClickGuideCoroutine(id));
    }

    private IEnumerator OnClickGuideCoroutine(int id)
    {
        yield return StartCoroutine(GetGuide(id));
    }

    private IEnumerator GetGuide(int id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"https://ganymede-dofus.com/api/guides/{id}"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log($"Received Guide {id}: " + webRequest.downloadHandler.text);
                
                guidesCurrentPath = Application.persistentDataPath + "/guides/";
                
                SaveGuideToFile(webRequest.downloadHandler.text, guidesCurrentPath);
            }
        }
    }

    private void SaveGuideToFile(string guideJson, string path)
    {
        if (string.IsNullOrEmpty(guideJson))
        {
            Debug.LogError("Guide JSON is empty or null.");
            return;
        }

        // Parse the JSON to get the guide name
        var guideObject = JObject.Parse(guideJson);
        var guideName = guideObject["name"]?.ToString();

        if (string.IsNullOrEmpty(guideName))
        {
            Debug.LogError("Guide name is not found in the JSON.");
            return;
        }

        string filePath = Path.Combine(path, guideName + ".json");

        try
        {
            Directory.CreateDirectory(path);
            File.WriteAllText(filePath, guideJson);
            Debug.Log("Guide saved to " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save guide: " + e.Message);
        }
    }

    public string GetDraftAndPublicListResponse()
    {
        return guidesDraftAndPublicListResponse;
    }

    public string GetCertifiedListResponse()
    {
        return guidesCertifiedListResponse;
    }
}
