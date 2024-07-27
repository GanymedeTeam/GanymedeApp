using UnityEngine;
using SatorImaging.AppWindowUtility;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.IO;

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
    public Toggle TravelCheckbox;
    public Toggle ShowCompletedGuidesCheckbox;
    public TMP_Text menuTitle;

    // Lock button
    public GameObject LockButton;
    public Sprite LockSprite;
    public Sprite UnlockSprite;
    bool isAppLocked = false;

    // Resolution fields
    public GameObject Xresolution;
    public GameObject Yresolution;

    public Slider opacitySlider;
    public Slider BgOpacitySlider;

    public bool isInteractiveMapActive;

    void Awake()
    {
        LoadPlayerPrefs();
    }

    void Start()
    {
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
        // Want travel in pos checkbox
        if (PlayerPrefs.HasKey("wantTravel"))
            TravelCheckbox.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1));
        else
            PlayerPrefs.SetInt("wantTravel", TravelCheckbox.isOn ? 1 : 0);

        // Want to show or not completed guides
        if (PlayerPrefs.HasKey("showCompletedGuides"))
            ShowCompletedGuidesCheckbox.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("showCompletedGuides", 1));
        else
            PlayerPrefs.SetInt("showCompletedGuides", ShowCompletedGuidesCheckbox.isOn ? 1 : 0);

        // Global opacity
        float opacity = PlayerPrefs.GetFloat("opacity", 1);
        opacitySlider.value = opacity;
        ChangeCanvasOpacity(opacity);

        // Guide Background Opacity
        float guideBgOpacity = PlayerPrefs.GetFloat("guideBgOpacity", 1);
        BgOpacitySlider.value = opacity;
        ChangeBgOpacity(guideBgOpacity);

        //Resolution
        windowWidth = PlayerPrefs.GetInt("XResolution", 0) * 50 + 300;
        windowHeight = PlayerPrefs.GetInt("YResolution", 0) * 50 + 450;
        Xresolution.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("XResolution", 0);
        Yresolution.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("YResolution", 0);
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

    public void RefreshGuideInteractiveMap()
    {
        SetMapFullscale(isInteractiveMapActive);
        ToggleInteractiveMap(isInteractiveMapActive);
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

    public void CopyTravelPositionSetting(bool wantTravel)
    {
        PlayerPrefs.SetInt("wantTravel", wantTravel ? 1 : 0);
    }

    public void ShowCompletedGuidesSetting(bool wantToShow)
    {
        PlayerPrefs.SetInt("showCompletedGuides", wantToShow ? 1 : 0);
    }

    public void SetXResolution(int x)
    {
        windowWidth = 300 + x * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("XResolution", x);
    }
    
    public void SetYResolution(int y)
    {
        windowHeight = 450 + y * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("YResolution", y);
    }

    public void ResetAppSize()
    {
        windowWidth = 300;
        windowHeight = 450;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
    }

    public void LockUnlockApp()
    {
        if ( isAppLocked )
        {
            // Unlock it
            this.GetComponent<WindowGrabber>().enabled = true;
            LockButton.transform.Find("LockImage").GetComponent<Image>().sprite = LockSprite;
            isAppLocked = false;
        }
        else
        {
            // Lock it
            this.GetComponent<WindowGrabber>().enabled = false;
            LockButton.transform.Find("LockImage").GetComponent<Image>().sprite = UnlockSprite;
            isAppLocked = true;
        }
        LockButton.GetComponent<Button>().interactable = false;
        LockButton.GetComponent<Button>().interactable = true;
    }
}
