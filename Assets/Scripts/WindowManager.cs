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
    public TMP_InputField windowWidthText;
    public TMP_InputField windowHeightText;
    public GameObject MinimizedWindows;
    public GameObject FullWindows;
    private GameObject SelectedWindow;
    public GameObject MainWindow;
    public GameObject travelWindow;
    public GameObject guideWindow;
    public GameObject settingsWindow;
    public GameObject downloadWindow;
    public GameObject notepadWindow;
    public CanvasGroup canvasGroup;
    public GameObject GuideBg;
    public TMP_Text menuTitle;

    public Slider opacitySlider;
    public Slider BgOpacitySlider;

    public bool isInteractiveMapActive;

    void Start()
    {
        LoadPlayerPrefs();
        AppWindowUtility.Transparent = true;
        AppWindowUtility.AlwaysOnTop = true;
        SelectedWindow = MainWindow;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        isInteractiveMapActive = true;
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
        float guideBgOpacity = PlayerPrefs.GetFloat("guideBgOpacity", 1);
        opacitySlider.value = opacity;
        BgOpacitySlider.value = opacity;
        ChangeCanvasOpacity(opacity);
        ChangeBgOpacity(opacity);
    }

    public void ToggleWindow()
    {
        AppWindowUtility.Transparent = !AppWindowUtility.Transparent;
    }

    public void MainWindowClicked()
    {
        if (SelectedWindow == MainWindow)
            return;
        menuTitle.text = "Ganymede";
        SelectedWindow.SetActive(false);
        SelectedWindow = MainWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void TravelWindowClicked()
    {
        if (SelectedWindow == travelWindow)
            return;
        menuTitle.text = "Autopilotage";
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void GuideWindowClicked()
    {
        if (SelectedWindow == guideWindow)
            return;
        menuTitle.text = "Guides";
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void NotepadWindowClicked()
    {
        if (SelectedWindow == notepadWindow)
            return;
        menuTitle.text = "Bloc-notes";
        SelectedWindow.SetActive(false);
        SelectedWindow = notepadWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void SettingsWindowClicked()
    {
        if (SelectedWindow == settingsWindow)
            return;
        menuTitle.text = "Options";
        SelectedWindow.SetActive(false);
        SelectedWindow = settingsWindow;
        SelectedWindow.SetActive(true);            
        ToggleInteractiveMap(false);
    }

    public void DownloadWindowClicked()
    {
        if (SelectedWindow == downloadWindow)
            return;
        menuTitle.text = "Téléchargement";
        SelectedWindow.SetActive(false);
        SelectedWindow = downloadWindow;
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

    public void SetMapFullscale(bool full)
    {
        if (full)
            guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 300f);
        else
            guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
    }

    public void ToggleGuideInteractiveMap()
    {
        isInteractiveMapActive = !isInteractiveMapActive;
        SetMapFullscale(isInteractiveMapActive);
        ToggleInteractiveMap(isInteractiveMapActive);
    }

    public void ToggleInteractiveMap(bool setActive)
    {
        if (setActive && guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin == new Vector2(0f, 300f))
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight + mapHeight);
        else
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }

    public void ChangeCanvasOpacity(float opacity)
    {
        canvasGroup.alpha = opacity;
        PlayerPrefs.SetFloat("opacity", opacity);
    }

    public void ChangeBgOpacity(float opacity)
    {
        var tempColor = GuideBg.GetComponent<Image>().color;
        tempColor.a = opacity;
        GuideBg.GetComponent<Image>().color = tempColor;
        PlayerPrefs.SetFloat("guideBgOpacity", opacity);
    }

    public void SetXResolution(int x)
    {
        windowWidth = 300 + x * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }
    
    public void SetYResolution(int y)
    {
        windowHeight = 450 + y * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }

    public void ResetAppSize()
    {
        windowWidth = 300;
        windowHeight = 450;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }
}
