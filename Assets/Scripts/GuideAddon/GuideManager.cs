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
        public string lang;
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
    public TMP_InputField searchBar;

    public Sprite fr_flag;
    public Sprite en_flag;
    public Sprite es_flag;
    public Sprite pt_flag;

    private string guides_url = $"{Constants.ganymedeWebUrl}/api/guides?status=";
    private string currentMenu = "root";

    public SaveManager saveManager;

    public void OnEnable()
    {
        StartCoroutine(saveManager.ProgressLoadJsonToClass());
        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width, content.GetComponent<GridLayoutGroup>().cellSize.y);
        if (currentMenu != "root")
            StartCoroutine(GetGuidesList());
    }

    public void OnDisable() 
    {
        DeleteGuidesFromView();
    }

    public void Update() 
    {
        if (!content.activeInHierarchy)
        {
            DeleteGuidesFromView();
            gameObject.GetComponent<PaginationHandler>().enabled = false;
        }
        else
            gameObject.GetComponent<PaginationHandler>().enabled = true;
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

    public void onClickGuidesGPList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "gp";
        StartCoroutine(GetGuidesList());
    }

    public void onClickGuidesPublicList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "public";
        StartCoroutine(GetGuidesList());
    }

    public void onClickGuidesDraftList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "draft";
        StartCoroutine(GetGuidesList());
    }

    public void onClickGuidesCertifiedList()
    {
        backButton.SetActive(true);
        rootMenu.SetActive(false);
        dlMenu.SetActive(true);
        currentMenu = "certified";
        StartCoroutine(GetGuidesList());
    }

    public void onClickReloadGuides()
    {
        StartCoroutine(GetGuidesList());
    }

    public void BackToRootMenu()
    {
        backButton.SetActive(false);
        dlMenu.SetActive(false);
        rootMenu.SetActive(true);
        currentMenu = "root";
    }

    public IEnumerator GetGuidesList()
    {
        string url = guides_url + currentMenu;
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

    private IEnumerator GetGuide(string url)
    {
        yield return saveManager.GuideLoadJsonToClass(int.Parse(url.Split('/').Last()));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string path;
                if (currentMenu == "gp")
                {
                    path = $"{Application.persistentDataPath}/guides/GP/{url.Split('/').Last()}.json";
                    if (!Directory.Exists($"{Application.persistentDataPath}/guides/GP"))
                        Directory.CreateDirectory($"{Application.persistentDataPath}/guides/GP");
                }
                else
                {
                    path = $"{Application.persistentDataPath}/guides/{url.Split('/').Last()}.json";
                }
                string jsonResponse = webRequest.downloadHandler.text;


                GuideEntry guide = JsonUtility.FromJson<GuideEntry>(jsonResponse);

                // Check if step is not overflowing
                SaveManager.SaveProgression saveProgress = saveManager.saveProgress;
                try
                {
                    if (guide.steps.Count() < saveProgress.guideProgress.First(e => e.id == guide.id).current_step)
                    {
                        int indexOfGuide = saveProgress.guideProgress.IndexOf(saveProgress.guideProgress.First(e => e.id == guide.id));
                        saveProgress.guideProgress[indexOfGuide].current_step = guide.steps.Count();
                        StartCoroutine(saveManager.ProgressSaveClassToJson());
                    }
                }
                catch{}

                // Checkboxes
                if (saveManager.saveGuide.steps.Count() != 0)
                {
                    saveManager.saveGuide = new SaveManager.SaveGuide();
                    StartCoroutine(saveManager.GuideSaveClassToJson(guide.id));
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
        StartCoroutine(GetGuidesList());
    }

    public void DownloadGuide(string path, string jsonContent)
    {
        System.IO.File.WriteAllText(path, jsonContent);
    }

    public void OnSearchBarValueChange()
    {
        gameObject.GetComponent<PaginationHandler>().currentPage = 1;
    }

    public void ShowAllGuidesInCurrentSection(ApiGuides listOfGuides)
    {
        string[] listOfLocalGuides = Directory.GetFiles(Application.persistentDataPath + "/guides/", "*.json", SearchOption.AllDirectories);
        for (int i = 0; i < listOfLocalGuides.Length; i++)
            listOfLocalGuides[i] = listOfLocalGuides[i].Split('/').Last().Replace(".json", "");

        content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(content.GetComponent<RectTransform>().rect.width, content.GetComponent<GridLayoutGroup>().cellSize.y);
        foreach (Transform child in content.transform) {
            if (child.name.Contains("guide_"))
                GameObject.Destroy(child.gameObject);
        }

        //filter list of downloadables
        List<ApiGuide> listOfShownGuides = new List<ApiGuide>();
        foreach (ApiGuide apiguide in listOfGuides.guides)
        {
            if (listOfLocalGuides.Any(name => name.Split('\\').Last().Equals(apiguide.id.ToString())))
                continue;
            if (searchBar.text == "")
                listOfShownGuides.Add(apiguide);
            else
            {
                if (
                    apiguide.id.ToString().ToLower().Contains(searchBar.text.ToLower()) ||
                    apiguide.name.ToLower().Contains(searchBar.text.ToLower()) ||
                    apiguide.user.name.ToLower().Contains(searchBar.text.ToLower())
                )
                    listOfShownGuides.Add(apiguide);
            }
        }

        // Use these for pagination
        gameObject.GetComponent<PaginationHandler>().totalElements = listOfShownGuides.Count();
        int maxElementsInPage = gameObject.GetComponent<PaginationHandler>().maxElementsInPage;
        int indexFirstElement = ( gameObject.GetComponent<PaginationHandler>().currentPage - 1 ) * maxElementsInPage;
        int indexLastElement = indexFirstElement + maxElementsInPage;
        int currentObjIndex = 0;
        //

        foreach (ApiGuide guide in listOfShownGuides)
        {
            if (currentObjIndex >= indexLastElement || currentObjIndex < indexFirstElement)
            {
                currentObjIndex++;
                continue;
            }
            currentObjIndex++;
            GameObject webGuide = Instantiate(webGuidePrefab, content.transform);
            webGuide.name = "guide_" + guide.id.ToString();
            webGuide.transform.SetParent(content.transform);
            webGuide.transform.Find("GuideInfo/GuideName").GetComponent<TMP_Text>().text = guide.name;
            webGuide.transform.Find("GuideInfo/GuideAuthor").GetComponent<TMP_Text>().text = "de: <color=#87cefa>" +  guide.user.name + "</color>";
            if (guide.user.is_certified == 1 || guide.user.is_admin == 1)
            {
                StartCoroutine(SetCertification(webGuide.transform.Find("GuideInfo/GuideAuthor").GetComponent<TMP_Text>()));
            }
            SetFlag(webGuide, guide.lang);
            webGuide.transform.Find("GuideInfo/GuideID").GetComponent<TMP_Text>().text = "id: <color=#dce775>" +  guide.id + "</color>";
            webGuide.transform.Find("GuideInfo/DownloadGuideButton").GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(GetGuide($"{Constants.ganymedeWebGuidesUrl}/{guide.id}")); });
        }
    }

    public IEnumerator SetFlag(GameObject webGuide, string lang)
    {
        if (lang == "fr")
            webGuide.transform.Find("GuideInfo/FlagInfo").GetComponent<Image>().sprite = fr_flag;
        else if (lang == "en")
            webGuide.transform.Find("GuideInfo/FlagInfo").GetComponent<Image>().sprite = en_flag;
        else if (lang == "es")
            webGuide.transform.Find("GuideInfo/FlagInfo").GetComponent<Image>().sprite = es_flag;
        else if (lang == "pt")
            webGuide.transform.Find("GuideInfo/FlagInfo").GetComponent<Image>().sprite = pt_flag;
        yield return 0;
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
