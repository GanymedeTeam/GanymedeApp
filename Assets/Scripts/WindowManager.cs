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
    private int windowWidth;
    private int windowHeight;
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

    // Lock button
    public GameObject LockButton;
    public Sprite LockSprite;
    public Sprite UnlockSprite;
    bool isAppLocked = false;

    // Resolution fields
    public GameObject Xresolution;
    public GameObject Yresolution;

    //Language field
    public GameObject language;

    public Slider opacitySlider;
    public Slider BgOpacitySlider;

    public bool isInteractiveMapActive;
    public bool keepInteractiveMapClosed;

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
        keepInteractiveMapClosed = false;
    }

    public void MinimizeApp()
    {
        FullWindows.SetActive(false);
        MinimizedWindows.SetActive(true);
        AppWindowUtility.SetScreenSize(50, 50);
    }

    public void MaximizeApp()
    {
        if (SelectedWindow == guideWindow && guideWindow.transform.Find("GuideDetailsMenu").gameObject.activeSelf)
        {
            if (guideWindow.transform.Find("GuideDetailsMenu").gameObject.activeSelf)
                InGuideRefreshInteractiveMap();
            else if (guideWindow.transform.Find("GuideSelectionMenu").gameObject.activeSelf)
                guideWindow.GetComponent<GuideMenu>().RemoveGuides();
        }
        else
        {
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        }
        FullWindows.SetActive(true);
        MinimizedWindows.SetActive(false);
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
        int xResolutionIndex = PlayerPrefs.GetInt("XResolution", 1);
        int yResolutionIndex = PlayerPrefs.GetInt("YResolution", 0);
        Xresolution.GetComponent<TMP_Dropdown>().value = xResolutionIndex;
        Yresolution.GetComponent<TMP_Dropdown>().value = yResolutionIndex;
        windowWidth = xResolutionIndex * 50 + 250;
        windowHeight = yResolutionIndex * 50 + 450;

        //Language
        int langIndex = PlayerPrefs.GetInt("lang", 0);
        language.GetComponent<TMP_Dropdown>().value = langIndex;
    }

    public void ToggleWindow()
    {
        AppWindowUtility.Transparent = !AppWindowUtility.Transparent;
    }

    public void MainWindowClicked()
    {
        if (SelectedWindow == MainWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = MainWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void TravelWindowClicked()
    {
        if (SelectedWindow == travelWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void GuideWindowClicked()
    {
        if (SelectedWindow == guideWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
        // ToggleInteractiveMap(false);
    }

    public void NotepadWindowClicked()
    {
        if (SelectedWindow == notepadWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = notepadWindow;
        SelectedWindow.SetActive(true);
        ToggleInteractiveMap(false);
    }

    public void SettingsWindowClicked()
    {
        if (SelectedWindow == settingsWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = settingsWindow;
        SelectedWindow.SetActive(true);            
        ToggleInteractiveMap(false);
    }

    public void DownloadWindowClicked()
    {
        if (SelectedWindow == downloadWindow)
            return;
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

    public void InGuideRefreshInteractiveMap()
    {
        if (keepInteractiveMapClosed)
        {
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
            guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
        }
        else
        {
            if (isInteractiveMapActive)
            {
                guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 300f);
                AppWindowUtility.SetScreenSize(windowWidth, windowHeight + mapHeight);
            }
            else
            {
                guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
            }
        }
    }

    public void InGuideToggleInteractiveMap()
    {
        isInteractiveMapActive = !isInteractiveMapActive;
        InGuideRefreshInteractiveMap();
    }

    public void ToggleInteractiveMap(bool setActive)
    {
        if (setActive)
        {
            guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 300f);
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight + mapHeight);
        }
        else
        {
            guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
            AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        }         
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
        windowWidth = 250 + x * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("XResolution", x);
    }
    
    public void SetYResolution(int y)
    {
        windowHeight = 450 + y * 50;
        AppWindowUtility.SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("YResolution", y);
    }

    public void SetLang(int langIndex)
    {
        PlayerPrefs.SetInt("lang", langIndex);
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
