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
    public GameObject settingsWindow;
    public CanvasGroup canvasGroup;

    public Slider opacitySlider;

    public bool isInteractiveMapActive = false;

    void Start()
    {
        LoadPlayerPrefs();
        AppWindowUtility.Transparent = true;
        AppWindowUtility.AlwaysOnTop = true;
        SelectedWindow = MainWindow;
        AppWindowUtility.SetScreenSize(300, 450);
    }

    private void LoadPlayerPrefs()
    {
        float opacity = PlayerPrefs.GetFloat("opacity", 1);
        opacitySlider.value = opacity;
        ChangeCanvasOpacity(opacity);
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

    public void SettingsWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = settingsWindow;
        SelectedWindow.SetActive(true);
        ActivateInteractiveMap(false);
    }

    public void DiscordButtonClicked()
    {
        Application.OpenURL("https://discord.gg/fxWuXB3dct");
    }

    public void TwitterButtonClicked()
    {
        Application.OpenURL("https://twitter.com/Pynx0");
    }

    public void WebsiteButtonClicked()
    {
        Application.OpenURL("https://ganymededofus.com");
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

    public void ChangeCanvasOpacity(float opacity)
    {
        canvasGroup.alpha = opacity;
        PlayerPrefs.SetFloat("opacity", opacity);
    }
}
