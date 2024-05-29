using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuideObject : MonoBehaviour
{
    public TMP_Text guideNameText;
    public string guideName;
    private string directoryPath;
    private bool isDirectory;
    public Image guideIcon;

    public void Initialize(string guideName, bool isDirectory, string directoryPath = null)
    {
        this.guideName = guideName;
        this.directoryPath = directoryPath;
        this.isDirectory = isDirectory;
        guideNameText.text = guideName;

        if (isDirectory)
        {
            guideIcon.enabled = true;
        }
        else
        {
            guideIcon.enabled = false;
        }
    }

    public void OpenGuide()
    {
        GuideMenu guideMenu = FindObjectOfType<GuideMenu>();

        if (isDirectory)
        {
            guideMenu.ReloadGuideList(directoryPath);
        }
        else
        {
            guideMenu.GuideDetailsMenu.SetActive(true);
            guideMenu.GuideSelectionMenu.SetActive(false);
            guideMenu.LoadGuide(guideName);
        }
    }
}
