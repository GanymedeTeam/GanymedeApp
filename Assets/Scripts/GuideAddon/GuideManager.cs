using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    [Serializable]
    public class ApiGuide
    {
        public int id;
        public int user_id;
        public string user;
        public string name;
        public string status;
        public string description;
        public string web_description;
        public string category;
        public string like;
        public string dislike;
        public string created_at;
        public string updated_at;
        public string deleted_at;
    }

    [Serializable]
    public class ApiGuides
    {
        public ApiGuide[] guides;
    }

    public GameObject content;
    public GameObject webGuidePrefab;
    public GameObject rootMenu;
    public GameObject scrollView;
    public GameObject backButton;

    public void onClickGuidesPublicList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        scrollView.SetActive(true);
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=public"));
    }

    public void onClickGuidesDraftList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        scrollView.SetActive(true);
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=draft"));
    }

    public void onClickGuidesCertifiedList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        scrollView.SetActive(true);
        StartCoroutine(GetGuidesList("https://ganymede-dofus.com/api/guides?status=certified"));
    }

    private IEnumerator GetGuidesList(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Received Guides List: " + jsonResponse);

                ShowAllGuidesInCurrentSection(JsonUtility.FromJson<ApiGuides>("{\"guides\":" + jsonResponse + "}"));
            }
        }
    }

    private IEnumerator GetGuide(string url, GameObject guideButton)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Received Guide: " + jsonResponse);

                DownloadGuide(url.Split('/')[url.Split('/').Length - 1], jsonResponse);
            }
        }
        yield return 0;
        guideButton.GetComponent<Button>().interactable = false;
        guideButton.GetComponent<Button>().interactable = true;
    }

    public void DownloadGuide(string name, string jsonContent)
    {
        System.IO.File.WriteAllText(Application.persistentDataPath + "/guides/" + name + ".json", jsonContent);
    }

    public void ShowAllGuidesInCurrentSection(ApiGuides listOfGuides)
    {
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width, content.GetComponent<GridLayoutGroup>().cellSize.y);
        foreach (Transform child in content.transform) {
            if (child.name.Contains("guide_"))
                GameObject.Destroy(child.gameObject);
        }
        foreach (ApiGuide guide in listOfGuides.guides)
        {
            GameObject webGuide = Instantiate(webGuidePrefab, content.transform);
            webGuide.name = "guide_" + guide.id.ToString();
            webGuide.transform.SetParent(content.transform);
            webGuide.transform.Find("GuideButton/GuideName").GetComponent<TMP_Text>().text = guide.name;
            webGuide.transform.Find("GuideButton/GuideAuthor").GetComponent<TMP_Text>().text = "de: <color=#87cefa>" +  guide.user + "</color>";
            webGuide.transform.Find("GuideButton/GuideID").GetComponent<TMP_Text>().text = "id: <color=#dce775>" +  guide.id + "</color>";
            webGuide.transform.Find("GuideButton").GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(GetGuide("https://ganymede-dofus.com/api/guides/" + guide.id, webGuide.transform.Find("GuideButton").gameObject)); });
        }
    }

    public void BackToRootMenu()
    {
        backButton.SetActive(false);
        scrollView.SetActive(false);
        rootMenu.SetActive(true);
    }
}
