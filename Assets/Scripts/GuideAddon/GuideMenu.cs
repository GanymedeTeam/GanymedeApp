using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Start()
    {
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

    public void ReloadGuideList()
    {
        RemoveGuides();
        
        string saveFilePath = Application.persistentDataPath + "/guides/";

        if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
        }

        var info = new DirectoryInfo(Application.persistentDataPath + "/guides");
        var fileInfo = info.GetFiles();

        Debug.Log("Files in guides folder: " + fileInfo.Length);

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

    public void LoadGuide(string guideName)
    {
        OpenedGuide = guideName;
        guideStepData = FileHandler.ReadListFromJSON<GuideEntry>("guides/" + guideName + ".json");
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