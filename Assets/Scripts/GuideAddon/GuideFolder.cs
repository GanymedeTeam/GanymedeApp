using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideFolder : MonoBehaviour
{
    public TMP_Text guideFolderText;
    public string guideFolderName;
    
    public void Initialize(string guideFolderName)
    {
        this.guideFolderName = guideFolderName;
        guideFolderText.text = guideFolderName;
    }

    public void OpenFolder()
    {
        GuideMenu guideMenu = FindObjectOfType<GuideMenu>();
        guideMenu.guidesCurrentPath += guideFolderName + "/";
        guideMenu.ReloadGuideList();
    }
}
