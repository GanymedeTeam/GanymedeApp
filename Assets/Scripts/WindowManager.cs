using UnityEngine;
using SatorImaging.AppWindowUtility;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WindowManager : MonoBehaviour
{
    private int windowWidth = 300;
    private int windowHeight = 450;
    private int mapHeight = 300;
    public int minHeight = 450;
    public int maxHeight = 1080;
    public int minWidth = 300;
    public int maxWidth = 1920;
    public TMP_InputField windowWidthText;
    public TMP_InputField windowHeightText;
    public GameObject MinimizedWindows;
    public GameObject FullWindows;
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

    public void MinimizeApp()
    {
        FullWindows.SetActive(false);
        MinimizedWindows.SetActive(true);
        AppWindowUtility.SetScreenSize(50, 50);
    }

    public void MaximizeApp()
    {
        FullWindows.SetActive(true);
        MinimizedWindows.SetActive(false);
        if (SelectedWindow == guideWindow)
        {
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight + mapHeight);
        }
        else
        {
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        }
    }

    public void CloseApp()
    {
        Application.Quit();
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
        ToggleInteractiveMap(false);
    }

    public void TravelWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void GuideWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(true);
    }

    public void MessageWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = messageWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void SettingsWindowClicked()
    {
        SelectedWindow.SetActive(false);
        SelectedWindow = settingsWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void DiscordButtonClicked()
    {
        Application.OpenURL("https://discord.gg/fxWuXB3dct");
    }

    public void TwitterButtonClicked()
    {
        Application.OpenURL("https://x.com/GanymedeDofus");
    }

    public void WebsiteButtonClicked()
    {
        Application.OpenURL("https://ganymede-dofus.com");
    }

    public void ToggleInteractiveMap(bool setActive)
    {
        if (setActive)
            AppWindowUtility.SetScreenSize(300, 750);
        else
            AppWindowUtility.SetScreenSize(300, 450);
    }

    public void ChangeCanvasOpacity(float opacity)
    {
        canvasGroup.alpha = opacity;
        PlayerPrefs.SetFloat("opacity", opacity);
    }

    public void SetResolution()
    {
        windowWidth = int.Parse(windowWidthText.text);
        windowHeight = int.Parse(windowHeightText.text);

        if (windowWidth < minWidth)
        {
            windowWidth = minWidth;
        }
        if (windowWidth > maxWidth)
        {
            windowWidth = maxWidth;
        }
        if (windowHeight < minHeight)
        {
            windowHeight = minHeight;
        }
        if (windowHeight > maxHeight)
        {
            windowHeight = maxHeight;
        }

        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }

    public void ResetAppSize()
    {
        windowWidth = 300;
        windowHeight = 450;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }
}
