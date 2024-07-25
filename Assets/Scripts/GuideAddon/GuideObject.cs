using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GuideObject : MonoBehaviour
{
    public TMP_Text guideNameText;
    public string guideName;
    public string id;
    public GameObject updtImage;
    public string path;
    
    public void Initialize(string guideName, string id, string path)
    {
        this.guideName = guideName;
        this.id = id;
        guideNameText.text = guideName;
        this.path = path;
    }

    public void OpenGuide()
    {
        GuideMenu guideMenu = FindObjectOfType<GuideMenu>();
        guideMenu.GuideDetailsMenu.SetActive(true);
        guideMenu.GuideSelectionMenu.SetActive(false);
        guideMenu.LoadGuide(id.ToString());
    }

    public void Start()
    {
        StartCoroutine(CheckForUpdate());
    }

    public IEnumerator UpdateGuideButton()
    {
        yield return StartCoroutine(UpdateMyGuide(path, id));
    }

    public IEnumerator UpdateMyGuide(string path, string id)
    {
        GuideEntry localGuide;
        try
        {
            string jsonToRead = File.ReadAllText(path + id + ".json");
            localGuide = JsonUtility.FromJson<GuideEntry>(jsonToRead);
        }
        catch
        {
            yield break;
        }
        using UnityWebRequest webRequest = UnityWebRequest.Get($"{Constants.ganymedeWebGuidesUrl}/{id}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
        {
            string jsonResponse = webRequest.downloadHandler.text;

            GuideEntry webGuide = JsonUtility.FromJson<GuideEntry>(jsonResponse);

            if (!JsonUtility.ToJson(localGuide).Equals(JsonUtility.ToJson(webGuide)))
            {
                System.IO.File.WriteAllText(path + id + ".json", jsonResponse);
                yield return new WaitForSeconds(2);
                int protection = 10;
                while (
                    !System.IO.File.Exists(path + id + ".json") ||
                    (DateTime.Today - System.IO.File.GetLastWriteTime(path + id + ".json")).TotalSeconds > 5
                )
                {
                    if (protection-- == 0)
                        break;
                    System.IO.File.WriteAllText(path + id + ".json", jsonResponse);
                    yield return new WaitForSeconds(2);
                }

                if (protection == 0)
                {
                    Debug.Log("Guide " + id + " could not be downloaded...");
                }
                else
                {
                    // Reset player prefs
                    if (PlayerPrefs.GetInt(id + "_currstep") >= webGuide.steps.Count())
                        PlayerPrefs.SetInt(id + "_currstep", webGuide.steps.Count() - 1);
                    Debug.Log("File " + id + " downloaded successfully!");
                    updtImage.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Guide " + id + " does not need update.");
            }
        }
        FindObjectOfType<GuideMenu>().OnClickReloadGuideList();
    }

    public IEnumerator CheckForUpdate()
    {
        GuideEntry localGuide;
        try
        {
            string jsonToRead = File.ReadAllText(path + id + ".json");
            localGuide = JsonUtility.FromJson<GuideEntry>(jsonToRead);
        }
        catch
        {
            yield break;
        }

        using UnityWebRequest webRequest = UnityWebRequest.Get($"{Constants.ganymedeWebGuidesUrl}/{id}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
        {
            string jsonResponse = webRequest.downloadHandler.text;

            GuideEntry webGuide = JsonUtility.FromJson<GuideEntry>(jsonResponse);
            if (!JsonUtility.ToJson(localGuide).Equals(JsonUtility.ToJson(webGuide)))
            {
                updtImage.SetActive(true);
            }
            else
            {
                updtImage.SetActive(false);
            }
        }

        yield return new WaitForSeconds(60);
        StartCoroutine(CheckForUpdate());
    }

    void Update()
    {
        // Check if guide is finished
        string[] steps = transform.Find("GuideInfo/GuideProgress").GetComponent<TMP_Text>().GetParsedText().Split('/');
        if (steps[0] == steps[1])
            transform.Find("GuideInfo/GuideFinishedImage").gameObject.SetActive(true);
        else
            transform.Find("GuideInfo/GuideFinishedImage").gameObject.SetActive(false);
    }
}
