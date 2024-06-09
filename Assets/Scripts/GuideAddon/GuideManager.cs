using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class GuideManager : MonoBehaviour
{
    public string guidesCurrentPath;

    private string guidesDraftAndPublicListResponse;
    private string guidesCertifiedListResponse;

    // API call api/guides dont return private guides
    public void onClickGuidesDraftAndPublicList(Action<string> callback)
    {
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=public", callback));
    }

    public void onClickGuidesCertifiedList(Action<string> callback)
    {
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=certified", callback));
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
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Received Guides List: " + jsonResponse);

                jsonResponse = FilterDownloadedGuides(jsonResponse);

                callback(jsonResponse);
            }
        }
    }

    private string FilterDownloadedGuides(string jsonResponse)
    {
        var guides = JArray.Parse(jsonResponse);
        var localGuideIds = GetLocalGuideIds();

        var filteredGuides = guides.Where(guide => !localGuideIds.Contains(guide["id"].ToString())).ToArray();

        return JArray.FromObject(filteredGuides).ToString();
    }

    private HashSet<string> GetLocalGuideIds()
    {
        var guideIds = new HashSet<string>();

        guidesCurrentPath = Application.persistentDataPath + "/guides/";

        if (Directory.Exists(guidesCurrentPath))
        {
            var files = Directory.GetFiles(guidesCurrentPath, "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var guide = JObject.Parse(json);
                    var guideId = guide["id"]?.ToString();

                    if (!string.IsNullOrEmpty(guideId))
                    {
                        guideIds.Add(guideId);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to read or parse guide file: " + e.Message);
                }
            }
        }

        return guideIds;
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

        // Parse the JSON to get the guide ID and name
        var guideObject = JObject.Parse(guideJson);
        var guideName = guideObject["name"]?.ToString();
        var guideId = guideObject["id"]?.ToString();

        if (string.IsNullOrEmpty(guideName) || string.IsNullOrEmpty(guideId))
        {
            Debug.LogError("Guide name or ID is not found in the JSON.");
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

    // Additional functions to get the stored responses
    public string GetDraftAndPublicListResponse()
    {
        return guidesDraftAndPublicListResponse;
    }

    public string GetCertifiedListResponse()
    {
        return guidesCertifiedListResponse;
    }
}
