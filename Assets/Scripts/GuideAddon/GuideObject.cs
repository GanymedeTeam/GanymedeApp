using System;
using System.IO;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using System.Globalization;

public class GuideObject : MonoBehaviour
{
    public TMP_Text guideNameText;
    public string guideName;
    public string id;
    public GameObject updtImage;
    public string path;

    private GuideEntry guideContent;
    
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
        try
        {
            string jsonToRead = File.ReadAllText(path + id + ".json");
            guideContent = JsonUtility.FromJson<GuideEntry>(jsonToRead);
        }
        catch
        {}
        StartCoroutine(FindObjectOfType<SaveManager>().GuideLoadJsonToClass(int.Parse(id)));
        StartCoroutine(CheckForUpdate());
    }

    public IEnumerator UpdateGuideButton()
    {
        yield return StartCoroutine(UpdateMyGuide(path, id));
    }

    public IEnumerator UpdateMyGuide(string path, string id)
    {
        if (!updtImage.activeSelf)
        {
            Debug.Log($"Guide {id} is currently up to date.");
            yield break;
        }
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
                    SaveManager.SaveProgression saveProgress = FindObjectOfType<SaveManager>().saveProgress;
                    int currStep;
                    try
                    {
                        currStep = saveProgress.guideProgress.First(e => e.id == int.Parse(id)).current_step;
                        int indexOfGuide = saveProgress.guideProgress.IndexOf(saveProgress.guideProgress.First(e => e.id == int.Parse(id)));
                        saveProgress.guideProgress[indexOfGuide].current_step = webGuide.steps.Count();
                    }
                    catch
                    {}

                    Debug.Log("File " + id + " downloaded successfully!");
                    updtImage.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Guide " + id + " does not need update.");
            }
        }
    }

    public IEnumerator CheckForUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        while (FindObjectOfType<GuideMenu>().apiGuides.guides.Count() == 0)
            yield return 0;
        GuideManager.ApiGuide apiGuide;
        try
        {
            apiGuide = FindObjectOfType<GuideMenu>().apiGuides.guides.First(g => g.id.ToString() == id);
        }
        catch
        {
            Debug.Log($"Guide {id} does not exist anymore");
            yield break;
        }

        if (ConvertToStandardFormat(apiGuide.updated_at) != ConvertToStandardFormat(guideContent.updated_at))
        {
            updtImage.SetActive(true);
        }
        else
            updtImage.SetActive(false);
    }

    private static string ConvertToStandardFormat(string dateStr)
    {
        if (DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss.ffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime date))
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        throw new FormatException("Le format de la date n'est pas reconnu : " + dateStr);
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
