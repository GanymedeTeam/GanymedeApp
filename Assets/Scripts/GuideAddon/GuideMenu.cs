using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GuideMenu : MonoBehaviour
{
    public List<GuideEntry> guideStepData = new List<GuideEntry>();
    public GameObject gridGameobject;
    public GameObject guideUIPrefab;
    public GameObject guideUIFolderPrefab;
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
    public TMP_InputField searchBar;

    public string guidesCurrentPath;

    void Start()
    {
        guidesCurrentPath = Application.persistentDataPath + "/guides/";
        ReloadGuideList();
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
        GUIUtility.systemCopyBuffer = Application.persistentDataPath + "/guides";
    }

    private DirectoryInfo[] GetGuidesFolders()
    {
        if (!Directory.Exists(Path.GetDirectoryName(guidesCurrentPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(guidesCurrentPath));
        }

        var info = new DirectoryInfo(guidesCurrentPath);

        var dirInfo = info.GetDirectories();

        if (searchBar.text != "")
        {
            dirInfo = dirInfo.Where(e => e.Name.ToLower().Contains(searchBar.text.ToLower())).ToArray();
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
        guideStepData = FileHandler.ReadListFromJSON<GuideEntry>(guidesCurrentPath.Replace(Application.persistentDataPath + "/", "") + guideName + ".json");
        Debug.Log(guideStepData.Count);
        guideNameText.text = guideName;

        //TODO : LOAD GUIDE PROGRESSION FROM SAVEFILE
        guideProgress = 0;
        LoadGuideProgression(guideName);

        //TODO : LOAD GUIDE DETAILS FROM RIGHT STEP
        guideIDText.text = (guideProgress+1).ToString() + "/" + guideStepData.Count;
        guideTitleText.text = guideStepData[guideProgress].title;
        guideDescriptionText.text = guideStepData[guideProgress].description;
        guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
    }

    public void GoToStep()
    {
        int step = int.Parse(guideGoToStepText.text);
        if(step > 0 && step <= guideStepData.Count)
        {
            GoToGuideStep(step-1);
        }
    }

    public void GoToGuideStep(int guideIndex)
    {
        guideProgress = guideIndex;
        guideIDText.text = (guideProgress+1).ToString() + "/" + guideStepData.Count;
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
            guideIDText.text = (guideProgress+1).ToString() + "/" + guideStepData.Count;
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
            guideIDText.text = (guideProgress+1).ToString() + "/" + guideStepData.Count;
            guideTitleText.text = guideStepData[guideProgress].title;
            guideDescriptionText.text = guideStepData[guideProgress].description;
            guideTravelPositionText.text = "Position : " + guideStepData[guideProgress].travelPosition;
            SaveGuideProgression();
            int posX = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[0]);
            int posY = int.Parse(guideStepData[guideProgress].travelPosition.Split(',')[1]);
            mapManager.updateMapFromStep(posX, posY, guideStepData[guideProgress].map);
        }
    }
}