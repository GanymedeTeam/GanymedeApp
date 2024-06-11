using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GuideMenu : MonoBehaviour
{

    public GameObject gridGameobject;
    public GameObject guideUIPrefab;
    public GameObject guideUIFolderPrefab;
    public GameObject GuideDetailsMenu;
    public GameObject GuideSelectionMenu;
    public GameObject StepContent;
    public string OpenedGuide;
    public TMP_Text currentPath;
    public int guideProgress = 0;
    public MapManager mapManager;
    public TMP_InputField guideGoToStepText;
    public TMP_InputField searchBar;
    public string guidesCurrentPath;
    public TMP_Text guideNameText;
    public TMP_Text stepNumberText;
    public TMP_Text stepTravelPositionText;
    public TMP_Text stepTitleText;
    public TMP_FontAsset textFont;

    [SerializeField]
    private string guideCurrID;
    [SerializeField]
    private GuideEntry guideInfos;

    void Start()
    {
        guidesCurrentPath = Application.persistentDataPath + "/guides/";
        ReloadGuideList();
    }

    public void CopyTravelPosition()
    {
        GUIUtility.systemCopyBuffer = "[" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_x + "]";
    }

    public void CopyGuideTravelPosition()
    {
        GUIUtility.systemCopyBuffer = "/travel [" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_y + "]";
    }

    public void OpenGuideFileStorage()
    {
        System.Diagnostics.Process.Start("explorer.exe", "/open," + Application.persistentDataPath.Replace('/', '\\') + "\\guides");
    }

    private DirectoryInfo[] GetGuidesFolders()
    {
        bool RecursiveSearch(DirectoryInfo dir)
        {
            bool matchedSearch = false;
            string[] files = Directory.GetFiles(@dir.FullName, "*.json", SearchOption.AllDirectories).Select(s => s.ToLowerInvariant()).ToArray();
            string[] folders = Directory.GetDirectories(@dir.FullName, "*", SearchOption.AllDirectories).Select(s => s.ToLowerInvariant()).ToArray();
            files = files.Select(e => e.Split('\\').Last()).ToArray();
            folders = folders.Select(e => e.Split('\\').Last()).ToArray();
            if (searchBar.text != "")
            {
                matchedSearch = Array.Exists(files, e => e.Contains(searchBar.text.ToLowerInvariant())) || Array.Exists(folders, e => e.Contains(searchBar.text.ToLowerInvariant()));
            }
            return matchedSearch;
        }

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));
        }

        var info = new DirectoryInfo(guidesCurrentPath);

        var dirInfo = info.GetDirectories();

        if (searchBar.text != "")
        {
            dirInfo = dirInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower()) || RecursiveSearch(e)).ToArray();
        }

        Debug.Log("Folders in guides folder: " + dirInfo.Length);

        return dirInfo;
    }

    private FileInfo[] GetGuidesInFolder()
    {

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));
        }

        var info = new DirectoryInfo(guidesCurrentPath);

        var fileInfo = info.GetFiles();

        Debug.Log("Files in guides folder: " + fileInfo.Length);

        if (searchBar.text != "")
        {
            fileInfo = fileInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower())).ToArray();
        }

        Debug.Log("Files shown: " + fileInfo.Length);

        return fileInfo;
    }

    public void ReloadGuideList()
    {
        void FormatCurrentPath()
        {
            string text = guidesCurrentPath;
            text = text.Replace(Application.persistentDataPath, "");
            text = text.Remove(0, 1);
            text = text.Remove(text.Length - 1, 1);
            bool isTruncated = false;
            int maxSize = 30;
            while (text.Length > maxSize)
            {
                if (text.Split('/').Count() == 1)
                {
                    break;
                }
                isTruncated = true;
                text = text.Substring(text.IndexOf('/', 2) + 1);
            }
            text = text.Replace("/", " <b><color=\"yellow\">></b></color> ");
            if (isTruncated)
            {
                text = "... <b><color=\"yellow\">></b></color> " + text;
            }
            Debug.Log(text);
            currentPath.text = text;
        }

        Debug.Log("Reloading list of guides!");
        RemoveGuides();

        var fileInfo = GetGuidesInFolder();
        var dirInfo = GetGuidesFolders();

        foreach (DirectoryInfo dir in dirInfo)
        {
            Debug.Log(dir);
            GameObject newGuideFolder = Instantiate(guideUIFolderPrefab, gridGameobject.transform);

            GuideFolder guideFolder = newGuideFolder.GetComponent<GuideFolder>();
            guideFolder.Initialize(dir.Name);
        }

        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                Debug.Log(file);
                GameObject newGuideObject = Instantiate(guideUIPrefab, gridGameobject.transform);

                GuideObject guideObject = newGuideObject.GetComponent<GuideObject>();
                guideObject.Initialize(file.Name.Replace(".json", ""));
            }
        }

        FormatCurrentPath();

    }

    void RemoveGuides()
    {
        foreach (Transform child in gridGameobject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BackToGuideSelection()
    {
        GuideDetailsMenu.SetActive(false);
        GuideSelectionMenu.SetActive(true);
    }

    public void BackToPreviousFolder()
    {
        Debug.Log("Leaving path " + guidesCurrentPath + " for previous folder...");
        string[] split = guidesCurrentPath.Split('/');

        if (split[split.Count() - 1] == "")
        {
            split = split.Take(split.Count() - 2).ToArray();
        }
        else
        {
            split = split.Take(split.Count() - 1).ToArray();
        }

        string newPath = String.Join("/", split) + "/";

        if (newPath.Contains(Application.persistentDataPath + "/guides/"))
        {
            guidesCurrentPath = newPath;
        }
        else
        {
            guidesCurrentPath = Application.persistentDataPath + "/guides/";
        }
        ReloadGuideList();
        Debug.Log("Currently in folder path " + guidesCurrentPath);
    }

    public void LoadGuide(string guideName)
    {
        OpenedGuide = guideName;
        string filePath = guidesCurrentPath + guideName + ".json";
        string jsonToRead = File.ReadAllText(filePath);
        guideInfos = JsonUtility.FromJson<GuideEntry>(jsonToRead);

        guideProgress = PlayerPrefs.GetInt(guideInfos.id.ToString() + "_currstep", -1);
        if (guideProgress == -1)
        {
            guideProgress = 0;
            PlayerPrefs.SetInt(guideInfos.id.ToString() + "_currstep", guideProgress);
        }

        guideNameText.text = guideInfos.name;
        guideCurrID = guideInfos.id.ToString();
        GoToGuideStep(guideProgress);
    }

    public void PublicGoToGuideStep(string guideIndex)
    {
        
        int step = Int32.Parse(guideIndex);
        if (step <= guideInfos.steps.Count() && step > 0)
            GoToGuideStep(step-1);
    }

    public void GoToGuideStep(int guideIndex)
    {
        foreach (Transform child in StepContent.transform) {
            if (child.name.Contains("Substep"))
                GameObject.Destroy(child.gameObject);
        }
        guideProgress = guideIndex;
        stepNumberText.text = (guideProgress+1).ToString() + "/" + guideInfos.steps.Count();
        stepTitleText.text = guideInfos.steps[guideProgress].name;
        stepTravelPositionText.text = "Position : <color=\"yellow\">[" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_x + "]</color>";
        ProcessSubSteps(guideInfos.steps[guideProgress].sub_steps);
        int posX = guideInfos.steps[guideProgress].pos_x;
        int posY = guideInfos.steps[guideProgress].pos_y;
        mapManager.updateMapFromStep(posX, posY, guideInfos.steps[guideProgress].map);
    }

    private void ProcessSubSteps(List<SubstepEntry> subentries)
    {
        int substepIndex = 0;
        Vector3[] v = new Vector3[4];
        StepContent.GetComponent<RectTransform>().GetWorldCorners(v);
        Vector3 topCornerPosition = v[1];
        foreach (SubstepEntry subentry in guideInfos.steps[guideProgress].sub_steps)
        {
            if (subentry.text == "")
                return;
            // Child gameobject
            GameObject subStepGameObject = new GameObject
            {
                name = "Substep " + (++substepIndex).ToString()
            };
            // Assign to parent
            subStepGameObject.transform.parent = StepContent.transform;
            // Create TMP Pro component and assign it to child
            TextMeshProUGUI substepTMP = subStepGameObject.AddComponent<TextMeshProUGUI>();
            substepTMP.fontSize = 16;
            ParseText(subentry.text);
            substepTMP.text = subentry.text;
            substepTMP.font = textFont;
            Vector2 textSize = substepTMP.GetPreferredValues();
            // Essayer de t rouver l'espacement avec la taille de la font+ taille espacement * nombre de lignes?
            subStepGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(StepContent.GetComponent<RectTransform>().rect.width, textSize.y);
            subStepGameObject.transform.position = topCornerPosition + new Vector3(StepContent.GetComponent<RectTransform>().rect.width / 2, - subStepGameObject.GetComponent<RectTransform>().rect.height / 2, 0);
            topCornerPosition += new Vector3(0, - subStepGameObject.GetComponent<RectTransform>().rect.height, 0);        
        }
    }

    private string ParseText(string text)
    {
        foreach (char c in text)
        {
            if (c == '<')
            {

            }
        }
        return text;
    }

    public void NextStep()
    {
        if (guideProgress < guideInfos.steps.Count() - 1)
        {
            GoToGuideStep(++guideProgress);
        }
    }

    public void PreviousStep()
    {
        if (guideProgress > 0)
        {
            GoToGuideStep(--guideProgress);
        }
    }
}