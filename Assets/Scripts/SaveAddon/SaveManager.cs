using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [Serializable]
    public class SaveStep
    {
        public int step_index;
        public List<int> idCheckboxTicked;
    }

    [Serializable]
    public class SaveGuide
    {
        public List<SaveStep> steps;
    }

    [Serializable]
    public class SaveProgression
    {
        [Serializable]
        public class GuideProgress
        {
            public int id;
            public int current_step;
        }
        public List<GuideProgress> guideProgress; 
    }

    public SaveGuide saveGuide;
    public SaveProgression saveProgress;
    public string saveName;

    private bool isWriting = false;

    void Awake()
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/Saves"))
            Directory.CreateDirectory($"{Application.persistentDataPath}/Saves");

        // Get current save
        saveName = PlayerPrefs.GetString("CharacterNameSave", "");

        // No current save
        if (saveName == "")
        {
            //No character is set yet
            //Check if folder is present
            string[] subFolders = Directory.GetDirectories($"{Application.persistentDataPath}/Saves");
            if (subFolders.Length > 0)
            {
                // Get first subfolder
                string firstSubFolder = subFolders[0];
                string firstSubFolderName = Path.GetFileName(firstSubFolder);
                PlayerPrefs.SetString("CharacterNameSave", firstSubFolderName);
                saveName = firstSubFolderName;
            }
            else
            {
                //Name it player, save it in playerprefs
                PlayerPrefs.SetString("CharacterNameSave", "Player");
                if (!Directory.Exists($"{Application.persistentDataPath}/Saves/Player"))
                    Directory.CreateDirectory($"{Application.persistentDataPath}/Saves/Player");

                // Migrate old saves
                if (File.Exists($"{Application.persistentDataPath}/progress.json"))
                    File.Move($"{Application.persistentDataPath}/progress.json", $"{Application.persistentDataPath}/Saves/Player/progress.json");
                if (Directory.Exists($"{Application.persistentDataPath}/guideSaves"))
                {
                    foreach (string file in Directory.GetFiles($"{Application.persistentDataPath}/guideSaves"))
                    {
                        string destFile = Path.Combine($"{Application.persistentDataPath}/Saves/Player", Path.GetFileName(file));
                        File.Move(file, destFile);
                    }
                    Directory.Delete($"{Application.persistentDataPath}/guideSaves", true);
                }

                saveName = "Player";
            }
        }
        if (!Directory.Exists($"{Application.persistentDataPath}/Saves/{saveName}"))
        {
            string[] subFolders = Directory.GetDirectories($"{Application.persistentDataPath}/Saves");
            if (subFolders.Length > 0)
            {
                // Get first subfolder
                string firstSubFolder = subFolders[0];
                string firstSubFolderName = Path.GetFileName(firstSubFolder);
                PlayerPrefs.SetString("CharacterNameSave", firstSubFolderName);
                saveName = firstSubFolderName;
            }
            else
            {
                //Name it player, save it in playerprefs
                PlayerPrefs.SetString("CharacterNameSave", "Player");
                saveName = "Player";
                if (!Directory.Exists($"{Application.persistentDataPath}/Saves/Player"))
                    Directory.CreateDirectory($"{Application.persistentDataPath}/Saves/Player");

                // Migrate old saves
                if (File.Exists($"{Application.persistentDataPath}/progress.json"))
                    File.Move($"{Application.persistentDataPath}/progress.json", $"{Application.persistentDataPath}/Saves/Player/progress.json");
                if (Directory.Exists($"{Application.persistentDataPath}/guideSaves"))
                {
                    foreach (string file in Directory.GetFiles($"{Application.persistentDataPath}/guideSaves"))
                    {
                        string destFile = Path.Combine($"{Application.persistentDataPath}/Saves/Player", Path.GetFileName(file));
                        File.Move(file, destFile);
                    }
                    Directory.Delete($"{Application.persistentDataPath}/guideSaves", true);
                }
            }
        }
        Debug.Log($"Loaded save: {saveName}");
    }

    public IEnumerator GuideSaveClassToJson(int id)
    {
        string path = $"{Application.persistentDataPath}/Saves/{saveName}/{id}.json";
        try
        {
            while (isWriting)
                continue;
            isWriting = true;
            string content = JsonUtility.ToJson(saveGuide);
            File.WriteAllText(path, content);
            isWriting = false;
        }
        catch
        {
            Debug.Log($"Couldn't open file at path {path}.");
        }
        yield return 0;
    }

    public IEnumerator GuideLoadJsonToClass(int id)
    {
        string content = "";
        string path = $"{Application.persistentDataPath}/Saves/{saveName}/{id}.json";
        try
        {
            content = File.ReadAllText(path);
        }
        catch{}
        saveGuide = JsonUtility.FromJson<SaveGuide>(content);
        saveGuide ??= new SaveGuide();
        saveGuide.steps ??= new List<SaveStep>();
        yield return 0;
    }

    public IEnumerator ProgressSaveClassToJson()
    {
        string path = $"{Application.persistentDataPath}/Saves/{saveName}/progress.json";
        try
        {
            while (isWriting)
                continue;
            isWriting = true;
            string content = JsonUtility.ToJson(saveProgress);
            File.WriteAllText(path, content);
            isWriting = false;
        }
        catch
        {
            Debug.Log($"Couldn't open file at path {path}.");
        }
        yield return 0;
    }

    public IEnumerator ProgressLoadJsonToClass()
    {
        string path = $"{Application.persistentDataPath}/Saves/{saveName}/progress.json";
        string content = "";
        try
        {
            content = File.ReadAllText(path);
        }
        catch
        {}
        saveProgress = JsonUtility.FromJson<SaveProgression>(content);
        saveProgress ??= new SaveProgression();
        saveProgress.guideProgress ??= new List<SaveProgression.GuideProgress>();
        yield return 0;
    }
}
