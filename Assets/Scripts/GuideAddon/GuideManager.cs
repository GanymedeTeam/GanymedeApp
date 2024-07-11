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
    public class UserGuide
    {
        public int id;
        public string name;
        public int is_admin;
        public int is_certified;
    }

    [Serializable]
    public class ApiGuide
    {
        public int id;
        public int user_id;
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
        public UserGuide user;
    }

    [Serializable]
    public class ApiGuides
    {
        public ApiGuide[] guides;
    }

    public GameObject content;
    public GameObject webGuidePrefab;
    public GameObject rootMenu;
    public GameObject dlMenu;
    public GameObject backButton;
    public Sprite spriteCertified;

    private const string guides_url = "https://ganymede-dofus.com/api/guides?status=";
    private string currentMenu = "root";

    public void OnEnable()
    {
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width, content.GetComponent<GridLayoutGroup>().cellSize.y);
        if (currentMenu != "root")
            StartCoroutine(GetGuidesList(guides_url + currentMenu));
    }

    public void OnDisable() 
    {
        DeleteGuidesFromView();
    }

    public void Update() 
    {
        if (!content.activeInHierarchy)
            DeleteGuidesFromView();
    }

    public void DeleteGuidesFromView()
    {
        foreach (Transform child in content.transform)
        {
            if (child.name.Contains("guide_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void onClickGuidesPublicList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "public";
        StartCoroutine(GetGuidesList(guides_url + currentMenu));
    }

    public void onClickGuidesDraftList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "draft";
        StartCoroutine(GetGuidesList(guides_url + currentMenu));
    }

    public void onClickGuidesCertifiedList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "certified";
        StartCoroutine(GetGuidesList(guides_url + currentMenu));
    }

    public void BackToRootMenu()
    {
        backButton.SetActive(false);
        dlMenu.SetActive(false);
        rootMenu.SetActive(true);
        currentMenu = "root";
    }

    private IEnumerator GetGuidesList(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string jsonResponse = webRequest.downloadHandler.text;
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

                string path = Application.persistentDataPath + "/guides/" + url.Split('/')[url.Split('/').Length - 1] + ".json" ;

                GuideEntry guide = JsonUtility.FromJson<GuideEntry>(jsonResponse);

                // Reset player prefs
                if (PlayerPrefs.GetInt(url.Split('/')[url.Split('/').Length - 1] + "_currstep") >= guide.steps.Count())
                    PlayerPrefs.SetInt(url.Split('/')[url.Split('/').Length - 1] + "_currstep", guide.steps.Count() - 1);
                // Checkboxes
                for (int stepIndex = 0; stepIndex < guide.steps.Count(); stepIndex++)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        PlayerPrefs.DeleteKey(url.Split('/')[url.Split('/').Length - 1] + "_cb_" + stepIndex + "_" + i);
                    }
                }

                DownloadGuide(path, jsonResponse);
                yield return new WaitForSeconds(2);
                int protection = 10;
                while (
                    !System.IO.File.Exists(path) ||
                    (DateTime.Today - System.IO.File.GetLastWriteTime(path)).TotalSeconds > 5
                )
                {
                    if (protection-- == 0)
                        break;
                    Debug.Log("Failed to download " + path + ", retrying...");
                    DownloadGuide(path, jsonResponse);
                    yield return new WaitForSeconds(2);
                }
            }
        }
        yield return 0;
        guideButton.GetComponent<Button>().interactable = false;
        guideButton.GetComponent<Button>().interactable = true;
    }

    public void DownloadGuide(string path, string jsonContent)
    {
        System.IO.File.WriteAllText(path, jsonContent);
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
            webGuide.transform.Find("GuideButton/GuideAuthor").GetComponent<TMP_Text>().text = "de: <color=#87cefa>" +  guide.user.name + "</color>";
            if (guide.user.is_certified == 1 || guide.user.is_admin == 1)
            {
                StartCoroutine(SetCertification(webGuide.transform.Find("GuideButton/GuideAuthor").GetComponent<TMP_Text>()));
            }
            webGuide.transform.Find("GuideButton/GuideID").GetComponent<TMP_Text>().text = "id: <color=#dce775>" +  guide.id + "</color>";
            webGuide.transform.Find("GuideButton").GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(GetGuide("https://ganymede-dofus.com/api/guides/" + guide.id, webGuide.transform.Find("GuideButton").gameObject)); });
        }
    }

    public IEnumerator SetCertification(TMP_Text text)
    {
        yield return 0;
        GameObject certification = new GameObject("Certification");
        Image certifImage = certification.AddComponent<Image>();
        certifImage.sprite = spriteCertified;
        RectTransform certifRT = certification.GetComponent<RectTransform>();
        certification.transform.SetParent(text.transform);
        certifRT.sizeDelta = new Vector2(12, 12);
        certifRT.anchoredPosition = new Vector3(
            text.textInfo.characterInfo[text.textInfo.characterCount - 1].bottomRight.x + 8,
            0,
            0
        );
    }
}
