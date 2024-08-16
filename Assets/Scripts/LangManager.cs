using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;
using UnityEngine.UI;

public class LangManager : MonoBehaviour
{
    [Serializable]
    public class Trad
    {
        [Serializable]
        public class TradFormat
        {
            public string fr;
            public string en;
            public string es;
            public string po;
        }

        [Serializable]
        public class MenuMain
        {
            public TradFormat MenuName;
            public TradFormat MenuDesc;
            public TradFormat Presentation;
            public TradFormat AlmanaxInfo;
            public TradFormat Affiliation;
            public TradFormat NotifUpdateTitle;
            public TradFormat NotifUpdateText;
            public TradFormat NotifUpdateButton;
        }

        [Serializable]
        public class MenuDownload
        {
            public TradFormat MenuTitle;
            public TradFormat RootMenuTitle;
            public TradFormat GPGuides;
            public TradFormat CertifiedGuides;
            public TradFormat PublicGuides;
            public TradFormat DraftGuides;
        }

        [Serializable]
        public class MenuNotepad
        {
            public TradFormat MenuTitle;
            public TradFormat MenuName;
            public TradFormat NoteEditionName;
            public TradFormat NoteContent;
        }

        [Serializable]
        public class MenuTravel
        {
            public TradFormat MenuTitle;
            public TradFormat Name;
        }

        [Serializable]
        public class MenuSettings
        {
            public TradFormat MenuTitle;
            public TradFormat GlobalOpacity;
            public TradFormat BgOpacity;
            public TradFormat Resolution;
            public TradFormat Width;
            public TradFormat Height;
            public TradFormat CopyTravel;
            public TradFormat ShowGuides;
            public TradFormat Language;
            public TradFormat Profiles;
            public TradFormat CreateProfiles;
            public TradFormat FontSizeText;
            public TradFormat NormalFontSize;
            public TradFormat BigFontSize;
            public TradFormat VeryBigFontSize;
            public TradFormat GiganticFontSize;
        }

        public TradFormat Searchbar;
        public TradFormat Guides;
        public MenuMain MainMenu;
        public MenuDownload DownloadMenu;
        public MenuNotepad NotepadMenu;
        public MenuTravel TravelMenu;
        public MenuSettings SettingsMenu;
    }

    public TMP_Text MenuName;

    // Dropdown Menu
    public TMP_Text ddMainMenu;
    public TMP_Text ddGuidesMenu;
    public TMP_Text ddDownloadMenu;
    public TMP_Text ddNotepadMenu;
    public TMP_Text ddTravelMenu;

    //MainMenu
    public TMP_Text MainMenuName;
    public TMP_Text MainMenuPresentation;
    public TMP_Text MainMenuAlmanaxInfoRetrieve;
    public TMP_Text MainMenuAffiliation;

    public TMP_Text NotifUpdateTitle;
    public TMP_Text NotifUpdateText;
    public Text NotifUpdateButtonText;

    //GuidesMenu
    public TMP_Text GuidesMenuSearchbar;

    //DownloadMenu
    public TMP_Text DownloadMenuRootName;
    public TMP_Text DownloadMenuDlName;
    public TMP_Text DownloadMenuGPGuides;
    public TMP_Text DownloadMenuCertifiedGuides;
    public TMP_Text DownloadMenuPublicGuides;
    public TMP_Text DownloadMenuDraftGuides;
    public TMP_Text DownloadMenuSearchbar;


    //NotepadMenu
    public TMP_Text NotepadMenuName;
    public TMP_Text NotepadMenuEditName;
    public TMP_Text NotepadContent;
    // NoteCopyButton handled in its

    //TravelMenu
    public TMP_Text TravelMenuName;

    //SettingsMenu
    public TMP_Text SettingsMenuTitle;
    public TMP_Text SettingsMenuGlobalOpacity;
    public TMP_Text SettingsMenuBgOpacity;
    public TMP_Text SettingsMenuResolution;
    public TMP_Text SettingsMenuWidth;
    public TMP_Text SettingsMenuHeight;
    public TMP_Text SettingsMenuCopyTravel;
    public TMP_Text SettingsMenuShowGuides;
    public TMP_Text SettingsMenuLanguage;
    public TMP_Text SettingsMenuProfiles;
    public TMP_Text SettingsMenuCreateProfiles;
    public TMP_Text SettingsMenuFontSizeText;
    public TMP_Dropdown SettingsMenuFontSizeDropdown;

    public Trad traductor;
    public string languageCode;

    void Awake()
    {
        TextAsset tradFile = Resources.Load<TextAsset>("trad");
        traductor = JsonUtility.FromJson<Trad>(tradFile.text);
        int lang = PlayerPrefs.GetInt("lang", 0);
        languageCode = lang switch
        {
            0 => "fr",
            1 => "en",
            2 => "es",
            3 => "po",
            _ => "fr",
        };

        MenuName.text = GetTranslationField(traductor.MainMenu.MenuName, languageCode);

        ddMainMenu.text = GetTranslationField(traductor.MainMenu.MenuName, languageCode);
        ddGuidesMenu.text = GetTranslationField(traductor.Guides, languageCode);
        ddDownloadMenu.text = GetTranslationField(traductor.DownloadMenu.MenuTitle, languageCode);
        ddNotepadMenu.text = GetTranslationField(traductor.NotepadMenu.MenuName, languageCode);
        ddTravelMenu.text = GetTranslationField(traductor.TravelMenu.MenuTitle, languageCode);

        MainMenuName.text = GetTranslationField(traductor.MainMenu.MenuDesc, languageCode);
        MainMenuPresentation.text = GetTranslationField(traductor.MainMenu.Presentation, languageCode);
        MainMenuAlmanaxInfoRetrieve.text = GetTranslationField(traductor.MainMenu.AlmanaxInfo, languageCode);
        MainMenuAffiliation.text = GetTranslationField(traductor.MainMenu.Affiliation, languageCode);

        NotifUpdateTitle.text = GetTranslationField(traductor.MainMenu.NotifUpdateTitle, languageCode);
        NotifUpdateText.text = GetTranslationField(traductor.MainMenu.NotifUpdateText, languageCode);
        NotifUpdateButtonText.text = GetTranslationField(traductor.MainMenu.NotifUpdateButton, languageCode);

        GuidesMenuSearchbar.text = GetTranslationField(traductor.Searchbar, languageCode);

        DownloadMenuRootName.text = GetTranslationField(traductor.DownloadMenu.RootMenuTitle, languageCode);
        DownloadMenuDlName.text = GetTranslationField(traductor.Guides, languageCode);
        DownloadMenuGPGuides.text = GetTranslationField(traductor.DownloadMenu.GPGuides, languageCode);
        DownloadMenuCertifiedGuides.text = GetTranslationField(traductor.DownloadMenu.CertifiedGuides, languageCode);
        DownloadMenuPublicGuides.text = GetTranslationField(traductor.DownloadMenu.PublicGuides, languageCode);
        DownloadMenuDraftGuides.text = GetTranslationField(traductor.DownloadMenu.DraftGuides, languageCode);
        DownloadMenuSearchbar.text = GetTranslationField(traductor.Searchbar, languageCode);

        NotepadMenuName.text = GetTranslationField(traductor.NotepadMenu.MenuName, languageCode);
        NotepadMenuEditName.text = GetTranslationField(traductor.NotepadMenu.NoteEditionName, languageCode);
        NotepadContent.text = GetTranslationField(traductor.NotepadMenu.NoteContent, languageCode);

        TravelMenuName.text = GetTranslationField(traductor.TravelMenu.MenuTitle, languageCode);

        SettingsMenuTitle.text = GetTranslationField(traductor.SettingsMenu.MenuTitle, languageCode);
        SettingsMenuGlobalOpacity.text = GetTranslationField(traductor.SettingsMenu.GlobalOpacity, languageCode);
        SettingsMenuBgOpacity.text = GetTranslationField(traductor.SettingsMenu.BgOpacity, languageCode);
        SettingsMenuResolution.text = GetTranslationField(traductor.SettingsMenu.Resolution, languageCode);
        SettingsMenuWidth.text = GetTranslationField(traductor.SettingsMenu.Width, languageCode);
        SettingsMenuHeight.text = GetTranslationField(traductor.SettingsMenu.Height, languageCode);
        SettingsMenuCopyTravel.text = GetTranslationField(traductor.SettingsMenu.CopyTravel, languageCode);
        SettingsMenuShowGuides.text = GetTranslationField(traductor.SettingsMenu.ShowGuides, languageCode);
        SettingsMenuLanguage.text = GetTranslationField(traductor.SettingsMenu.Language, languageCode);
        SettingsMenuProfiles.text = GetTranslationField(traductor.SettingsMenu.Profiles, languageCode);
        SettingsMenuCreateProfiles.text = GetTranslationField(traductor.SettingsMenu.CreateProfiles, languageCode);
        SettingsMenuFontSizeText.text = GetTranslationField(traductor.SettingsMenu.FontSizeText, languageCode);
        SettingsMenuFontSizeDropdown.options[0].text = GetTranslationField(traductor.SettingsMenu.NormalFontSize, languageCode);
        SettingsMenuFontSizeDropdown.options[1].text = GetTranslationField(traductor.SettingsMenu.BigFontSize, languageCode);
        SettingsMenuFontSizeDropdown.options[2].text = GetTranslationField(traductor.SettingsMenu.VeryBigFontSize, languageCode);
        SettingsMenuFontSizeDropdown.options[3].text = GetTranslationField(traductor.SettingsMenu.GiganticFontSize, languageCode);
        SettingsMenuFontSizeDropdown.RefreshShownValue();
    }

    public string GetTranslationField(Trad.TradFormat translation, string languageCode)
    {
        Type type = translation.GetType();
        FieldInfo field = type.GetField(languageCode, BindingFlags.Public | BindingFlags.Instance);
        return field.GetValue(translation) as string;
    }
}
