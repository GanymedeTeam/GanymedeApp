using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuideMenu : MonoBehaviour
{
    public List<GuideEntry> guideStepData = new List<GuideEntry>();
    public GameObject gridGameobject;
    public GameObject guideUIPrefab;
    public GameObject GuideDetailsMenu;
    public GameObject GuideSelectionMenu;
    public string OpenedGuide;
    public TMP_Text guideNameText;
    public TMP_Text guideIDText;
    public TMP_Text guideTravelPositionText;
    public TMP_Text guideTitleText;
    public TMP_Text guideDescriptionText;
    public int guideProgress = 0;
    public MapManager mapManager;
    public TMP_InputField guideGoToStepText;
    public TMP_InputField guideSearchText;
    public Button backButton;

    private string input;

    void Start()
    {
        ReloadGuideList();
        guideSearchText.onValueChanged.AddListener(delegate { FilterGuides(); });
        backButton.gameObject.SetActive(false);
    }

    void LoadGuideProgression(string guideName)
    {
        //TODO : LOAD GUIDE PROGRESSION FROM SAVEFILE
        string saveFilePath = Application.persistentDataPath + "/guideprogression/" + guideName + "_progression";
        if (File.Exists(saveFilePath))
        {
            string saveData = File.ReadAllText(saveFilePath);
            guideProgress = int.Parse(saveData);
        }
        else
        {
            guideProgress = 0;
        }
    }

    void SaveGuideProgression()
    {
        //TODO : SAVE GUIDE PROGRESSION TO SAVEFILE

        string saveFilePath = Application.persistentDataPath + "/guideprogression/" + OpenedGuide + "_progression";
        if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
        }
        File.WriteAllText(saveFilePath, guideProgress.ToString());
    }

    public void CopyTravelPosition()
    {
        GUIUtility.systemCopyBuffer = "/travel " + guideStepData[guideProgress].travelPosition;
    }

    public void CopyGuideFileStorage()
    {
        string path = Application.persistentDataPath + "/guides";
        if (Directory.Exists(path))
        {
            OpenFolder(path);
        }
        else
        {
            UnityEngine.Debug.LogError("Directory does not exist: " + path);
        }
    }

    private void OpenFolder(string path)
    {
        #if UNITY_EDITOR
            System.Diagnostics.Process.Start("explorer.exe", path.Replace("/", "\\"));
        #elif UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path.Replace("/", "\\"));
        #else
            UnityEngine.Debug.LogError("Opening folder is not supported on this platform.");
        #endif
    }

    public void ReloadGuideList(string directoryPath = null)
    {
        RemoveGuides();

        string saveFilePath = directoryPath ?? Application.persistentDataPath + "/guides/";

        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }

        var info = new DirectoryInfo(saveFilePath);
        var fileInfo = info.GetFiles();
        var dirInfo = info.GetDirectories();

        Debug.Log("Files in guides folder: " + fileInfo.Length);
        Debug.Log("Directories in guides folder: " + dirInfo.Length);

        // Directories
        foreach (DirectoryInfo dir in dirInfo)
        {
            Debug.Log(dir);
            GameObject newGuideObject = Instantiate(guideUIPrefab, gridGameobject.transform);

            GuideObject guideObject = newGuideObject.GetComponent<GuideObject>();
            guideObject.Initialize(dir.Name, true, dir.FullName);
        }

        // Files
        foreach (FileInfo file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                Debug.Log(file);
                GameObject newGuideObject = Instantiate(guideUIPrefab, gridGameobject.transform);

                GuideObject guideObject = newGuideObject.GetComponent<GuideObject>();
                guideObject.Initialize(file.Name.Replace(".json", ""), false, null);
            }
        }

        // Render Back button if in subdirectories
        backButton.gameObject.SetActive(directoryPath != null);
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
        ReloadGuideList();
    }

    public void LoadGuide(string guideName)
    {
        OpenedGuide = guideName;

        // Looking for guide in directory and subdirectory
        string guidesDirectory = Application.persistentDataPath + "/guides";
        string[] guideFiles = Directory.GetFiles(guidesDirectory, guideName + ".json", SearchOption.AllDirectories);

        if (guideFiles.Length == 0)
        {
            Debug.LogError("Fichier de guide non trouvé: " + guideName);
            return;
        }

        string guideFilePath = guideFiles[0]; // Supposons qu'il y a un seul fichier avec le nom exact
        guideFilePath = guideFilePath.Replace("\\", "/");
        // Assurez-vous que le chemin commence par /guides
        int guidesIndex = guideFilePath.IndexOf("/guides");
        if (guidesIndex != -1)
        {
            guideFilePath = guideFilePath.Substring(guidesIndex);
        }
        else
        {
            Debug.LogError("Le chemin du guide ne contient pas /guides: " + guideFilePath);
            return;
        }

        guideStepData = FileHandler.ReadListFromJSON<GuideEntry>(guideFilePath);
        guideNameText.text = guideName;

        //TODO : LOAD GUIDE PROGRESSION FROM SAVEFILE
        guideProgress = 0;
        LoadGuideProgression(guideName);

        //TODO : LOAD GUIDE DETAILS FROM RIGHT STEP
        guideIDText.text = (guideProgress + 1).ToString() + "/" + guideStepData.Count;
        guideTitleText.text = guideStepData[guideProgress].title;
        guideDescriptionText.text = guideStepData[guideProgress].description;
        guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
    }

    public void GoToStep()
    {
        int step = int.Parse(guideGoToStepText.text);
        if (step > 0 && step <= guideStepData.Count)
        {
            GoToGuideStep(step - 1);
        }
    }

    public void GoToGuideStep(int guideIndex)
    {
        guideProgress = guideIndex;
        guideIDText.text = (guideProgress + 1).ToString() + "/" + guideStepData.Count;
        guideTitleText.text = guideStepData[guideProgress].title;
        guideDescriptionText.text = guideStepData[guideProgress].description;
        guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
        SaveGuideProgression();
        int posX = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[0]);
        int posY = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[1]);
        mapManager.updateMapFromStep(posX, posY, guideStepData[guideProgress].map);
    }

    public void NextStep()
    {
        if (guideProgress < guideStepData.Count - 1)
        {
            guideProgress++;
            guideIDText.text = (guideProgress + 1).ToString() + "/" + guideStepData.Count;
            guideTitleText.text = guideStepData[guideProgress].title;
            guideDescriptionText.text = guideStepData[guideProgress].description;
            guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
            SaveGuideProgression();
            int posX = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[0]);
            int posY = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[1]);
            mapManager.updateMapFromStep(posX, posY, guideStepData[guideProgress].map);
        }
    }

    public void PreviousStep()
    {
        if (guideProgress > 0)
        {
            guideProgress--;
            guideIDText.text = (guideProgress + 1).ToString() + "/" + guideStepData.Count;
            guideTitleText.text = guideStepData[guideProgress].title;
            guideDescriptionText.text = guideStepData[guideProgress].description;
            guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
            SaveGuideProgression();
            int posX = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[0]);
            int posY = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[1]);
            mapManager.updateMapFromStep(posX, posY, guideStepData[guideProgress].map);
        }
    }

    public void FilterGuides()
    {
        string filterText = guideSearchText.text?.ToLower();

        if (string.IsNullOrEmpty(filterText))
        {
            ReloadGuideList(); // If null reload guide list
            return;
        }

        RemoveGuides();

        string saveFilePath = Application.persistentDataPath + "/guides/";
        if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
        }

        var info = new DirectoryInfo(Application.persistentDataPath + "/guides");
        var fileInfo = info.GetFiles("*.json", SearchOption.AllDirectories); //Search look on subdirectories

        Debug.Log("Fichiers dans le dossier des guides et sous-dossiers: " + fileInfo.Length);

        foreach (FileInfo file in fileInfo)
        {
            if (file.Name.ToLower().Contains(filterText))
            {
                Debug.Log(file);
                GameObject newGuideObject = Instantiate(guideUIPrefab, gridGameobject.transform);

                GuideObject guideObject = newGuideObject.GetComponent<GuideObject>();
                guideObject.Initialize(file.Name.Replace(".json", ""), false, file.FullName);
            }
        }
    }
}
