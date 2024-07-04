using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideObject : MonoBehaviour
{
    public TMP_Text guideNameText;
    public string guideName;
    public string id;
    
    public void Initialize(string guideName, string id)
    {
        this.guideName = guideName;
        this.id = id;
        guideNameText.text = guideName;
    }

    public void OpenGuide()
    {
        GuideMenu guideMenu = FindObjectOfType<GuideMenu>();
        guideMenu.GuideDetailsMenu.SetActive(true);
        guideMenu.GuideSelectionMenu.SetActive(false);
        guideMenu.LoadGuide(id.ToString());
    }
}
