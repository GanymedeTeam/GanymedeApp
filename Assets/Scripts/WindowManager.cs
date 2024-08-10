using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.IO;
using Kirurobo;
using UnityEngine.SceneManagement;

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

    // menu dropdown
    public GameObject menuDropdown;
    public GameObject toggleMenuDropdownButton;

    // current menu shown
    public GameObject currentMenuIcon;
    public GameObject currentMenuText;

    // lang manager
    public LangManager langManager;

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

    //Buttons
    public Button mainMenuButton;
    public Button guideMenuButton;
    public Button downloadMenuButton;
    public Button notepadMenuButton;
    public Button travelMenuButton;

    private Button currentSelectedMenu;

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
        currentSelectedMenu = mainMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
        StartCoroutine(ResizeAtStart());
    }

    private IEnumerator ResizeAtStart()
    {
        yield return 0;
        SetScreenSize(windowWidth, windowHeight);
        float x = PlayerPrefs.GetFloat("windowXPosition", -1f);
        float y = PlayerPrefs.GetFloat("windowYPosition", -1f);
        if (x != -1 && y != -1)
        {
            UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
            u.windowPosition = new Vector2(x, y);
        }

    }

    void SetScreenSize(int x, int y)
    {
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        u.windowSize = new Vector2(x, y);
    }

    public void MinimizeApp()
    {
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        u.windowPosition += new Vector2(0, windowHeight + (mapState ? mapHeight : 0) - 50);
        FullWindows.SetActive(false);
        MinimizedWindows.SetActive(true);
        SetScreenSize(50, 50);
    }

    public void MaximizeApp()
    {
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        u.windowPosition += new Vector2(0, - (windowHeight + (mapState ? mapHeight : 0) - 50));
        SetScreenSize(windowWidth, windowHeight + (mapState ? mapHeight : 0));
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
        currentMenuIcon.GetComponent<Image>().sprite = mainMenuButton.transform.Find("Image").GetComponent<Image>().sprite;
        currentMenuText.GetComponent<TMP_Text>().text = langManager.GetTranslationField(langManager.traductor.MainMenu.MenuName, langManager.languageCode);
        menuDropdown.SetActive(false);
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(false);
        currentSelectedMenu = mainMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
        SelectedWindow.SetActive(false);
        SelectedWindow = MainWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
    }

    public void TravelWindowClicked()
    {
        if (SelectedWindow == travelWindow)
            return;
        currentMenuIcon.GetComponent<Image>().sprite = travelMenuButton.transform.Find("Image").GetComponent<Image>().sprite;
        currentMenuText.GetComponent<TMP_Text>().text = langManager.GetTranslationField(langManager.traductor.TravelMenu.MenuTitle, langManager.languageCode);
        menuDropdown.SetActive(false);
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(false);
        currentSelectedMenu = travelMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
        SelectedWindow.SetActive(false);
        SelectedWindow = travelWindow;
        SelectedWindow.SetActive(true);
        ToggleMap(false);
    }

    public void GuideWindowClicked()
    {
        if (SelectedWindow == guideWindow)
            return;
        currentMenuIcon.GetComponent<Image>().sprite = guideMenuButton.transform.Find("Image").GetComponent<Image>().sprite;
        currentMenuText.GetComponent<TMP_Text>().text = langManager.GetTranslationField(langManager.traductor.Guides, langManager.languageCode);
        menuDropdown.SetActive(false);
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(false);
        currentSelectedMenu = guideMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
        SelectedWindow.SetActive(false);
        SelectedWindow = guideWindow;
        SelectedWindow.SetActive(true);
    }

    public void NotepadWindowClicked()
    {
        if (SelectedWindow == notepadWindow)
            return;
        currentMenuIcon.GetComponent<Image>().sprite = notepadMenuButton.transform.Find("Image").GetComponent<Image>().sprite;
        currentMenuText.GetComponent<TMP_Text>().text = langManager.GetTranslationField(langManager.traductor.NotepadMenu.MenuName, langManager.languageCode);
        menuDropdown.SetActive(false);
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(false);
        currentSelectedMenu = notepadMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
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
        currentMenuIcon.GetComponent<Image>().sprite = downloadMenuButton.transform.Find("Image").GetComponent<Image>().sprite;
        currentMenuText.GetComponent<TMP_Text>().text = langManager.GetTranslationField(langManager.traductor.DownloadMenu.MenuTitle, langManager.languageCode);
        menuDropdown.SetActive(false);
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(false);
        currentSelectedMenu = downloadMenuButton;
        currentSelectedMenu.transform.Find("HighlightedBorder").gameObject.SetActive(true);
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

    public void ToggleMenuDropdown()
    {
        toggleMenuDropdownButton.GetComponent<Button>().interactable = false;
        menuDropdown.SetActive(!menuDropdown.activeSelf);
    }

    public IEnumerator EnableDropdownDelay()
    {
        yield return new WaitForSeconds(0.2f);
        toggleMenuDropdownButton.GetComponent<Button>().interactable = true;
    }

    public void EnableDropdown()
    {
        StartCoroutine(EnableDropdownDelay());
    }

    public IEnumerator EmergencyResetWindowPosition()
    {
        yield return new WaitForSeconds(5);
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.N))
        {
            UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
            u.windowPosition = new Vector2(300, 300);
        }
        isRunningEmergency = false;
    }

    private bool isRunningEmergency = false;
    void Update()
    {
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.N) && !isRunningEmergency)
        {
            isRunningEmergency = true;
            StartCoroutine(EmergencyResetWindowPosition());
        }
    }

    void OnApplicationQuit()
    {
        UniWindowController u = UIWindowController.GetComponent<UniWindowController>();
        PlayerPrefs.SetFloat("windowXPosition", u.windowPosition.x);
        PlayerPrefs.SetFloat("windowYPosition", u.windowPosition.y);
    }
}
