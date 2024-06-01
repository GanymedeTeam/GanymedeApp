using UnityEngine;
using SatorImaging.AppWindowUtility;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WindowManager : MonoBehaviour
{
    private GameObject SelectedWindow;
    public GameObject MainWindow;
    public GameObject travelWindow;
    public GameObject guideWindow;
    public GameObject messageWindow;

    public bool isInteractiveMapActive = false;

    void Start()
    {
        AppWindowUtility.Transparent = true;
        AppWindowUtility.AlwaysOnTop = true;
        SelectedWindow = MainWindow;
        AppWindowUtility.SetScreenSize(300, 450);
    }

    public void ToggleWindow()
    {
        AppWindowUtility.Transparent = !AppWindowUtility.Transparent;
    }

    public void MainWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = MainWindow;
        SelectedWindow.SetActive(true);
        ActivateInteractiveMap(false);
    }

    public void TravelWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ActivateInteractiveMap(false);
    }

    public void GuideWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
        ActivateInteractiveMap(true);
    }

    public void MessageWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = messageWindow;
        SelectedWindow.SetActive(true);
        ActivateInteractiveMap(false);
    }

    public void ActivateInteractiveMap(bool isActivate)
    {
        if (isActivate)
        {
            if (!isInteractiveMapActive)
            {
                isInteractiveMapActive = true;
                AppWindowUtility.SetScreenSize(300, 750);
            }
        }
        else
        {
            if (isInteractiveMapActive)
            {
                isInteractiveMapActive = false;
                AppWindowUtility.SetScreenSize(300, 450);
            }
        }
    }
}