using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.IO;
using Kirurobo;

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

    //Window utility
    public GameObject UIWindowController;

    //Grabbers
    public GameObject FullSizeGrabber;
    public GameObject minSizeGrabber;

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

    public bool mapState;
    public bool toggleMapState = false;
    public bool keepInteractiveMapClosed = false;

    void Awake()
    {
        LoadPlayerPrefs();
    }

    void Start()
    {
#if !UNITY_EDITOR
        UIWindowController.SetActive(true);
#endif
        SelectedWindow = MainWindow;
        StartCoroutine(ResizeAtStart());
    }

    private IEnumerator ResizeAtStart()
    {
        yield return 0;
        SetScreenSize(windowWidth, windowHeight);
    }

    void SetScreenSize(int x, int y)
    {
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        u.windowSize = new Vector2(x, y);
    }

    public void MinimizeApp()
    {
        FullWindows.SetActive(false);
        MinimizedWindows.SetActive(true);
        SetScreenSize(50, 50);
    }

    public void MaximizeApp()
    {
        ToggleMap(mapState);
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

    public void MainWindowClicked()
    {
        if (SelectedWindow == MainWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = MainWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
    }

    public void TravelWindowClicked()
    {
        if (SelectedWindow == travelWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
    }

    public void GuideWindowClicked()
    {
        if (SelectedWindow == guideWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
    }

    public void NotepadWindowClicked()
    {
        if (SelectedWindow == notepadWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = notepadWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
    }

    public void SettingsWindowClicked()
    {
        if (SelectedWindow == settingsWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = settingsWindow;
        SelectedWindow.SetActive(true);            
        ToggleMap(false);
    }

    public void DownloadWindowClicked()
    {
        if (SelectedWindow == downloadWindow)
            return;
        SelectedWindow.SetActive(false);
        SelectedWindow = downloadWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
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

    public void InGuideRefreshInteractiveMap()
    {
        ToggleMap(mapState);
    }

    public void ToggleMap(bool setActive)
    {
        if (!setActive) //Close map in any cases
        {
            if (mapState) //Map was previously opened, so offset it
            {
                guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                SetScreenSize(windowWidth, windowHeight);
                UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
                u.windowPosition += new Vector2(0, mapHeight);
            }
        }
        else //Open map if allowed
        {
            if (keepInteractiveMapClosed) // no map
                return;
            if (!mapState) //map was previously closed, so open it
            {
                guideWindow.transform.Find("GuideDetailsMenu").GetComponent<RectTransform>().offsetMin = new Vector2(0f, 300f);
                SetScreenSize(windowWidth, windowHeight + mapHeight);
                UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
                u.windowPosition += new Vector2(0, -mapHeight);
            }
        }
        mapState = setActive;
    }

    public void InGuideToggleInteractiveMap()
    {
        toggleMapState = !mapState;
        ToggleMap(toggleMapState);
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
        SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("XResolution", x);
    }
    
    public void SetYResolution(int y)
    {
        int offset = windowHeight;
        windowHeight = 450 + y * 50;
        offset -= windowHeight;
        SetScreenSize(windowWidth, windowHeight);
        PlayerPrefs.SetInt("YResolution", y);
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        u.windowPosition += new Vector2(0, offset);
    }

    public void SetLang(int langIndex)
    {
        PlayerPrefs.SetInt("lang", langIndex);
    }

    public void LockUnlockApp()
    {
        if ( isAppLocked )
        {
            // Unlock it
            FullSizeGrabber.SetActive(true);
            minSizeGrabber.SetActive(true);
            LockButton.transform.Find("LockImage").GetComponent<Image>().sprite = LockSprite;
            isAppLocked = false;
        }
        else
        {
            // Lock it
            FullSizeGrabber.SetActive(false);
            minSizeGrabber.SetActive(false);
            LockButton.transform.Find("LockImage").GetComponent<Image>().sprite = UnlockSprite;
            isAppLocked = true;
        }
        LockButton.GetComponent<Button>().interactable = false;
        LockButton.GetComponent<Button>().interactable = true;
    }
}
