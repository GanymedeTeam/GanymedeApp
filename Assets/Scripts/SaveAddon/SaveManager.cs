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
    
    private bool isWriting = false;

    public IEnumerator GuideSaveClassToJson(int id)
    {
        string path = $"{Application.persistentDataPath}/guideSaves/{id}.json";
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
        string path = $"{Application.persistentDataPath}/guideSaves/{id}.json";
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
        string path = $"{Application.persistentDataPath}/progress.json";
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
        string path = $"{Application.persistentDataPath}/progress.json";
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
