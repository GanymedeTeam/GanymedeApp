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
    public GameObject SubstepImagePrefab;
    public string OpenedGuide;
    public TMP_Text currentPath;
    public int guideProgress = 0;
    public MapManager mapManager;
    public TMP_InputField guideGoToStepText;
    public TMP_InputField searchBar;
    public string guidesCurrentPath;
    public TMP_Text guideNameText;
    public TMP_Text stepMaxNumberText;
    public TMP_Text stepTravelPositionText;
    public TMP_FontAsset textFont;
    public TMP_InputField inputStep;
    public Scrollbar guideMenuScrollbar;

    [SerializeField]
    private GuideEntry guideInfos;

    void Awake()
    {
        guidesCurrentPath = Application.persistentDataPath + "/guides/";
    }

    public void OnEnable()
    {
        gridGameobject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gridGameobject.GetComponent<RectTransform>().rect.width, gridGameobject.GetComponent<GridLayoutGroup>().cellSize.y);
        gameObject.GetComponent<PaginationHandler>().enabled = true;
        StartCoroutine(ReloadGuideList());
    }

    public void OnDisable() {
        gameObject.GetComponent<PaginationHandler>().enabled = false;
    }

    public void DeleteGuidesFromView()
    {
        foreach (Transform child in gridGameobject.transform)
        {
            if (child.name.Contains("guide_") || child.name.Contains("folder_"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void CopyPosition()
    {
        GUIUtility.systemCopyBuffer = (Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1)) ? "/travel " : "") + "[" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_y + "]";
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
            string[] allFileNames = Directory.GetFiles(@dir.FullName, "*.json", SearchOption.AllDirectories);
            string[] allDirNames = Directory.GetDirectories(@dir.FullName, "*", SearchOption.AllDirectories);
            string[] allGuideNameInfo;

            List<string> tmpAllGuideNameInfo = new List<string>();

            foreach (string s in allFileNames)
            {
                string fileContent = String.Join("", System.IO.File.ReadAllLines(s));
                GuideEntry fileContentSerialized = JsonUtility.FromJson<GuideEntry>(fileContent);
                tmpAllGuideNameInfo.Add(fileContentSerialized.name);
            }
            allGuideNameInfo = tmpAllGuideNameInfo.ToArray();

            string[] files = allFileNames.Select(s => s.ToLowerInvariant()).ToArray().Select(e => e.Split('\\').Last()).ToArray();
            string[] folders = allDirNames.Select(s => s.ToLowerInvariant()).ToArray().Select(e => e.Split('\\').Last()).ToArray();

            if (searchBar.text != "")
                matchedSearch = Array.Exists(files, e => e.Contains(searchBar.text.ToLowerInvariant())) 
                || Array.Exists(folders, e => e.Contains(searchBar.text.ToLowerInvariant()))
                || Array.Exists(allGuideNameInfo, e => e.ToLower().Contains(searchBar.text.ToLowerInvariant()));
            return matchedSearch;
        }

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));

        DirectoryInfo info = new DirectoryInfo(guidesCurrentPath);

        DirectoryInfo[] dirInfo = info.GetDirectories();

        if (searchBar.text != "")
            dirInfo = dirInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower()) || RecursiveSearch(e)).ToArray();

        return dirInfo;
    }

    private FileInfo[] GetGuidesInFolder()
    {

        FileInfo[] finalFileInfo;

        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));

        DirectoryInfo info = new DirectoryInfo(guidesCurrentPath);

        FileInfo[] fileInfo = info.GetFiles();
        List<string> guideNameInfo = new List<string>();
        try
        {
            // for old json, use a try/catch
            foreach (var f in fileInfo)
            {
                string fileContent = String.Join("", System.IO.File.ReadAllLines(guidesCurrentPath + f.Name));
                GuideEntry fileContentSerialized = JsonUtility.FromJson<GuideEntry>(fileContent);
                guideNameInfo.Add(fileContentSerialized.name);
            }
        }
        catch
        {
            guideNameInfo = Enumerable.Repeat(string.Empty, fileInfo.Length).ToList();
        }

        finalFileInfo = fileInfo;

        if (searchBar.text != "")
        {
            List<FileInfo> tmpFinalFileInfo = new List<FileInfo>();
            for (int index = 0; index < fileInfo.Length; index++)
            {
                if (fileInfo[index].Name.ToLower().Contains(searchBar.text.ToLower())
                || guideNameInfo[index].ToLower().Contains(searchBar.text.ToLower()))
                    tmpFinalFileInfo.Add(fileInfo[index]);
            }
            finalFileInfo = tmpFinalFileInfo.ToArray();
        }

        return finalFileInfo;
    }

    public void OnClickReloadGuideList()
    {
        StartCoroutine(ReloadGuideList());
    }

    public void OnSearchBarValueChange()
    {
        gameObject.GetComponent<PaginationHandler>().currentPage = 1;
    }

    public IEnumerator ReloadGuideList()
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
            currentPath.text = text;
        }

        RemoveGuides();

        yield return 0;

        var fileInfo = GetGuidesInFolder();
        var dirInfo = GetGuidesFolders();


        // Use these for pagination
        gameObject.GetComponent<PaginationHandler>().totalElements = dirInfo.Count() + fileInfo.Count();
        int maxElementsInPage = gameObject.GetComponent<PaginationHandler>().maxElementsInPage;
        int indexFirstElement = ( gameObject.GetComponent<PaginationHandler>().currentPage - 1 ) * maxElementsInPage;
        int indexLastElement = indexFirstElement + maxElementsInPage;
        int currentObjIndex = 0;
        //
        foreach (DirectoryInfo dir in dirInfo)
        {
            if (currentObjIndex >= indexLastElement || currentObjIndex < indexFirstElement)
            {
                currentObjIndex++;
                continue;
            }
            currentObjIndex++;
            GameObject newGuideFolder = Instantiate(guideUIFolderPrefab, gridGameobject.transform);
            newGuideFolder.name = "folder_" + currentObjIndex.ToString();

            GuideFolder guideFolder = newGuideFolder.GetComponent<GuideFolder>();
            guideFolder.Initialize(dir.Name);
            newGuideFolder.transform.Find("FolderButton/OpenFolderButton").GetComponent<Button>().onClick.AddListener(
                    delegate {
                        newGuideFolder.GetComponent<GuideFolder>().OpenFolder();
                    }
                );
        }

        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                if (currentObjIndex >= indexLastElement || currentObjIndex < indexFirstElement)
                {
                    currentObjIndex++;
                    continue;
                }
                try
                {
                    string fileContent = String.Join("", System.IO.File.ReadAllLines(guidesCurrentPath + file.Name));
                    GuideEntry fileContentSerialized = JsonUtility.FromJson<GuideEntry>(fileContent);
                    int stepProgress = PlayerPrefs.GetInt(fileContentSerialized.id.ToString() + "_currstep", 0) + 1;
                    int totalSteps = fileContentSerialized.steps.Count();
                    if (stepProgress == totalSteps && PlayerPrefs.GetInt("showCompletedGuides", 1) == 0)
                        continue;
                    currentObjIndex++;
                    // Instanciate gameobject guide
                    GameObject newGuideObject = Instantiate(guideUIPrefab, gridGameobject.transform);
                    newGuideObject.name = "guide_" + file.Name.Replace(".json", "");
                    newGuideObject.transform.SetParent(gridGameobject.transform);
                    newGuideObject.GetComponent<GuideObject>().Initialize(fileContentSerialized.name, file.Name.Replace(".json", ""), guidesCurrentPath);
                    newGuideObject.transform.Find("GuideInfo/GuideID").GetComponent<TMP_Text>().text = "id: <color=#dce775>" +  file.Name.Replace(".json", "") + "</color>";
                    newGuideObject.transform.Find("GuideInfo/GuideProgress").GetComponent<TMP_Text>().text = "<color=yellow>" +  stepProgress + "</color>" + "/" + totalSteps;
                    newGuideObject.transform.Find("GuideInfo/OpenGuideButton").GetComponent<Button>().onClick.AddListener(
                        delegate {
                            newGuideObject.GetComponent<GuideObject>().OpenGuide();
                        }
                    );
                    newGuideObject.transform.Find("GuideInfo/UpdateGuideButton").GetComponent<Button>().onClick.AddListener(
                        delegate {
                            StartCoroutine(UpdateSingleGuide(newGuideObject));
                        }
                    );
                }
                catch
                {
                }
            }
        }
        FormatCurrentPath();

        yield return 0;

    }

    private IEnumerator UpdateSingleGuide(GameObject guideObject)
    {
        var scrollBarValue = guideMenuScrollbar.value;
        yield return StartCoroutine(guideObject.GetComponent<GuideObject>().UpdateGuideButton());
        yield return StartCoroutine(ReloadGuideList());
        guideMenuScrollbar.value = scrollBarValue;

    }

    void RemoveGuides()
    {
        foreach (Transform child in gridGameobject.transform)
            Destroy(child.gameObject);
    }

    public void BackToGuideSelection()
    {
        FindObjectOfType<WindowManager>().ToggleInteractiveMap(false);
        StartCoroutine(ReloadGuideList());
        GuideDetailsMenu.SetActive(false);
        GuideSelectionMenu.SetActive(true);
    }

    public void BackToPreviousFolder()
    {
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
        StartCoroutine(ReloadGuideList());
    }

    public void LoadGuide(string guideName)
    {
        OpenedGuide = guideName;
        string[] listOfIdGuides = Directory.GetFiles(Application.persistentDataPath + "/guides/", $"{guideName}.json", SearchOption.AllDirectories);
        if (listOfIdGuides.Count() == 0)
            return;
        string filePath = listOfIdGuides[0];
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
        try
        {
            int step = Int32.Parse(guideIndex);
            if (step <= guideInfos.steps.Count() && step > 0)
                GoToGuideStep(step-1);
        }
        catch
        {
            return;
        }
    }

    public void GoToGuideStep(int guideIndex)
    {
        if (guideInfos.steps.Count() < guideProgress + 1)
            return;
        foreach (Transform child in StepContent.transform) {
            if (child.name.Contains("Substep"))
                GameObject.Destroy(child.gameObject);
        }
        guideProgress = guideIndex;
        PlayerPrefs.SetInt(guideInfos.id.ToString() + "_currstep", guideProgress);
        inputStep.text = (guideProgress + 1).ToString();
        stepMaxNumberText.text = guideInfos.steps.Count().ToString();
        stepTravelPositionText.text = "<color=\"yellow\">[" + guideInfos.steps[guideProgress].pos_x + "," + guideInfos.steps[guideProgress].pos_y + "]</color>";
        ProcessSubSteps(guideInfos.steps[guideProgress].sub_steps);
        int posX = guideInfos.steps[guideProgress].pos_x;
        int posY = guideInfos.steps[guideProgress].pos_y;
        mapManager.updateMapFromStep(posX, posY, guideInfos.steps[guideProgress].map);
        StepContent.transform.parent.parent.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1f;
        FindObjectOfType<WindowManager>().RefreshGuideInteractiveMap();
    }

    private void ProcessSubSteps(List<SubstepEntry> subentries)
    {
        int substepIndex = 0;
        Regex CombinedRegex = new Regex(@"<checkbox>(.*?)</checkbox>|<image url=""([^""]+)"" ratio=""([\d.]+)"">");

        List<(string content, string type, float ratio)> entities = new List<(string content, string type, float ratio)>();

        foreach (SubstepEntry subentry in subentries)
        {
            ExtractEntitiesFromText(subentry.text, entities);
        }

        InstantiateSubstepGameObjects(entities, ref substepIndex);

        void ExtractEntitiesFromText(string text, List<(string content, string type, float ratio)> entities)
        {
            int lastIndex = 0;

            foreach (Match match in CombinedRegex.Matches(text))
            {
                AddTextBeforeMatch(text, match, entities, ref lastIndex);

                if (match.Groups[1].Success) // Checkbox
                {
                    entities.Add((match.Groups[1].Value, "checkbox", 0f));
                }
                else if (match.Groups[2].Success && match.Groups[3].Success) // Image with ratio
                {
                    entities.Add((match.Groups[2].Value, "image", float.Parse(match.Groups[3].Value)));
                }

                lastIndex = match.Index + match.Length;
            }

            AddRemainingText(text, lastIndex, entities);
        }

        void AddTextBeforeMatch(string text, Match match, List<(string content, string type, float ratio)> entities, ref int lastIndex)
        {
            string contentBeforeMatch = text.Substring(lastIndex, match.Index - lastIndex);
            if (!string.IsNullOrEmpty(contentBeforeMatch))
            {
                entities.Add((contentBeforeMatch, "text", 0f));
            }
        }

        void AddRemainingText(string text, int lastIndex, List<(string content, string type, float ratio)> entities)
        {
            string remainingText = text.Substring(lastIndex);
            if (!string.IsNullOrEmpty(remainingText))
            {
                entities.Add((remainingText, "text", 0f));
            }
        }

        void InstantiateSubstepGameObjects(List<(string content, string type, float ratio)> entities, ref int substepIndex)
        {
            foreach (var (content, type, ratio) in entities)
            {
                if (string.IsNullOrEmpty(content) && type != "image")
                {
                    continue;
                }

                GameObject subStepGameObject = InstantiateGameObject(content, type, ratio, ref substepIndex);
                if (type == "checkbox")
                {
                    ConfigureCheckbox(subStepGameObject, substepIndex);
                }
            }
        }

        GameObject InstantiateGameObject(string content, string type, float ratio, ref int substepIndex)
        {
            GameObject subStepGameObject;

            if (type == "image")
            {
                subStepGameObject = Instantiate(SubstepImagePrefab, StepContent.transform);
                var imageComponent = subStepGameObject.GetComponent<GuideSubstepImage>();
                imageComponent.SetImageUrl(content);
                imageComponent.SetImageRatio(ratio);
            }
            else
            {
                subStepGameObject = Instantiate(SubstepPrefab, StepContent.transform);
                subStepGameObject.name = $"Substep {++substepIndex}";
                subStepGameObject.transform.Find("Text").GetComponent<TMP_Text>().text = content;
            }

            return subStepGameObject;
        }

        void ConfigureCheckbox(GameObject subStepGameObject, int substepIndex)
        {
            var toggleObject = subStepGameObject.transform.Find("Toggle").gameObject;
            toggleObject.SetActive(true);

            var toggle = toggleObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate { SaveCheckboxStates(); });

            string playerPrefKey = $"{guideInfos.id}_cb_{guideProgress}_{substepIndex}";
            toggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt(playerPrefKey, 0));
        }
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
            if (child.Find("Toggle").gameObject.activeSelf)
            {
                PlayerPrefs.SetInt(
                    $"{guideInfos.id}_cb_{guideProgress}_{child.name[child.name.Length - 1]}",
                    child.transform.Find("Toggle").gameObject.GetComponent<Toggle>().isOn ? 1 : 0
                );
            }
        }
    }

    public void PreviousStep()
    {
        if (guideProgress > 0)
            GoToGuideStep(--guideProgress);
    }

    void Update()
    {
        if ( transform.Find("GuideSelectionMenu").gameObject.activeSelf )
            gameObject.GetComponent<PaginationHandler>().enabled = true;
        else
            gameObject.GetComponent<PaginationHandler>().enabled = false;

        if (!inputStep.isFocused && inputStep.text != (guideProgress + 1).ToString())
        {
            inputStep.text = (guideProgress + 1).ToString();
        }
    }
}