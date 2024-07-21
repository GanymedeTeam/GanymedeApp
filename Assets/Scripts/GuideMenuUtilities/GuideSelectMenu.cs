using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;

public class GuideSelectMenu : MonoBehaviour
{
    private GameObject guideMenu;

    void Awake()
    {
        guideMenu = transform.parent.gameObject;
    }

    void OnEnable()
    {
        guideMenu.GetComponent<GuideMenu>().OnClickReloadGuideList();
    }
}