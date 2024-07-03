using UnityEngine;
using SatorImaging.AppWindowUtility;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject winManager;

    void OnEnable()
    {
        winManager.GetComponent<WindowManager>().ToggleInteractiveMap(true);
    }
}
