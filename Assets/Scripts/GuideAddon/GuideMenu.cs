using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class GuideMenu : MonoBehaviour
{

    public GameObject gridGameobject;
    public GameObject guideUIPrefab;
    public GameObject guideUIFolderPrefab;
    public GameObject GuideDetailsMenu;
    public GameObject GuideSelectionMenu;
    public GameObject StepContent;
    public GameObject SubstepPrefab;
    public GameObject CheckboxPrefab;
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
                matchedSearch = Array.Exists(files, e => e.Contains(searchBar.text.ToLowerInvariant())) || Array.Exists(folders, e => e.Contains(searchBar.text.ToLowerInvariant()));
            return matchedSearch;
        }

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));

        var info = new DirectoryInfo(guidesCurrentPath);

        var dirInfo = info.GetDirectories();

        if (searchBar.text != "")
            dirInfo = dirInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower()) || RecursiveSearch(e)).ToArray();

        Debug.Log("Folders in guides folder: " + dirInfo.Length);

        return dirInfo;
    }

    private FileInfo[] GetGuidesInFolder()
    {

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));

        var info = new DirectoryInfo(guidesCurrentPath);

        var fileInfo = info.GetFiles();

        Debug.Log("Files in guides folder: " + fileInfo.Length);

        if (searchBar.text != "")
            fileInfo = fileInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower())).ToArray();

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
                    break;
                isTruncated = true;
                text = text.Substring(text.IndexOf('/', 2) + 1);
            }
            text = text.Replace("/", " <b><color=\"yellow\">></b></color> ");
            if (isTruncated)
                text = "... <b><color=\"yellow\">></b></color> " + text;
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
            Destroy(child.gameObject);
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
            split = split.Take(split.Count() - 2).ToArray();
        else
            split = split.Take(split.Count() - 1).ToArray();

        string newPath = String.Join("/", split) + "/";

        if (newPath.Contains(Application.persistentDataPath + "/guides/"))
            guidesCurrentPath = newPath;
        else
            guidesCurrentPath = Application.persistentDataPath + "/guides/";
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
            if (child.name.Contains("Substep") || child.name.Contains("Checkbox"))
                GameObject.Destroy(child.gameObject);
        }
        guideProgress = guideIndex;
        stepNumberText.text = (guideProgress+1).ToString() + "/" + guideInfos.steps.Count();
        stepTitleText.text = guideInfos.steps[guideProgress].name;
        stepTravelPositionText.text = "<color=\"yellow\">[" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_x + "]</color>";
        ProcessSubSteps(guideInfos.steps[guideProgress].sub_steps);
        int posX = guideInfos.steps[guideProgress].pos_x;
        int posY = guideInfos.steps[guideProgress].pos_y;
        mapManager.updateMapFromStep(posX, posY, guideInfos.steps[guideProgress].map);
    }

    private void ProcessSubSteps(List<SubstepEntry> subentries)
    {
        IEnumerator SetSubStepPosition(List<GameObject> substepList)
        {
            yield return 0;
            float offsetSubstepPosition = 0f;
            foreach (GameObject substep in substepList)
            {
                substep.transform.position += new Vector3(0, offsetSubstepPosition, 0);
                offsetSubstepPosition -= substep.GetComponent<RectTransform>().rect.height;
            }
            StepContent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -offsetSubstepPosition);
        }

        int substepIndex = 0;
        Regex CheckboxRegex = new Regex(@"<checkbox>(.+?)</checkbox>");
        List<(string, bool)> entities = new List<(string, bool)>();
        List<GameObject> subStepsList = new List<GameObject>();
        foreach (SubstepEntry subentry in subentries)
        {
            string text = subentry.text;
            foreach (Match checkboxMatch in CheckboxRegex.Matches(subentry.text))
            {
                string[] disassembled_text = text.Split(new string[] { checkboxMatch.Value }, StringSplitOptions.None);
                entities.Add((disassembled_text[0], false));
                entities.Add((checkboxMatch.Groups[1].Value, true));
                text = text.Replace(disassembled_text[0] + checkboxMatch.Value, "");
            }
            entities.Add((text, false));
        }
        foreach ((string, bool) entity in entities)
        {
            if (entity.Item1 == "")
                continue;
            if (entity.Item2 == true)
            {
                GameObject checkboxGameObject = Instantiate(CheckboxPrefab, StepContent.transform);
                subStepsList.Add(checkboxGameObject);
                checkboxGameObject.name = "Checkbox " + (++substepIndex).ToString();
                checkboxGameObject.transform.GetChild(0).gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate { SaveCheckboxStates(); });
                checkboxGameObject.transform.GetChild(0).gameObject.GetComponent<Toggle>().isOn = Convert.ToBoolean(PlayerPrefs.GetInt(guideInfos.id.ToString() + "_cb" + guideProgress + checkboxGameObject.name[checkboxGameObject.name.Length - 1]));
                checkboxGameObject.transform.SetParent(StepContent.transform);
                checkboxGameObject.GetComponent<TMP_Text>().text = entity.Item1;
            }
            else if (entity.Item2 == false)
            {
                GameObject subStepGameObject = Instantiate(SubstepPrefab, StepContent.transform);
                subStepsList.Add(subStepGameObject);
                subStepGameObject.name = "Substep " + (++substepIndex).ToString();
                subStepGameObject.transform.SetParent(StepContent.transform);
                subStepGameObject.GetComponent<TMP_Text>().text = entity.Item1;
            }
        }
        StartCoroutine(SetSubStepPosition(subStepsList));
    }

    public void NextStep()
    {
        if (guideProgress < guideInfos.steps.Count() - 1)
        {
            GoToGuideStep(++guideProgress);
        }
    }

    public void SaveCheckboxStates()
    {
        foreach (Transform child in StepContent.transform)
        {
            if (child.name.Contains("Checkbox"))
                PlayerPrefs.SetInt(guideInfos.id.ToString() + "_cb" + guideProgress + child.name[child.name.Length - 1], child.transform.GetChild(0).gameObject.GetComponent<Toggle>().isOn ? 1 : 0);
        }
    }

    public void PreviousStep()
    {
        if (guideProgress > 0)
            GoToGuideStep(--guideProgress);
    }
}