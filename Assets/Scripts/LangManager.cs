using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;

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
            public TradFormat DiscordButton;
            public TradFormat Affiliation;
        }

        [Serializable]
        public class MenuDownload
        {
            public TradFormat MenuTitle;
            public TradFormat RootMenuTitle;
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
        }

        public TradFormat Searchbar;
        public TradFormat Guides;
        public MenuMain MainMenu;
        public MenuDownload DownloadMenu;
        public MenuNotepad NotepadMenu;
        public MenuTravel TravelMenu;
        public MenuSettings SettingsMenu;
    }

    //MainMenu
    public TMP_Text MainMenuTopName;
    public TMP_Text MainMenuName;
    public TMP_Text MainMenuPresentation;
    public TMP_Text MainMenuDiscordButton;
    public TMP_Text MainMenuAffiliation;

    //GuidesMenu
    public TMP_Text GuidesMenuTopName;
    public TMP_Text GuidesMenuSearchbar;

    //DownloadMenu
    public TMP_Text DownloadMenuTopName;
    public TMP_Text DownloadMenuRootName;
    public TMP_Text DownloadMenuDlName;
    public TMP_Text DownloadMenuCertifiedGuides;
    public TMP_Text DownloadMenuPublicGuides;
    public TMP_Text DownloadMenuDraftGuides;
    public TMP_Text DownloadMenuSearchbar;


    //NotepadMenu
    public TMP_Text NotepadMenuTopName;
    public TMP_Text NotepadMenuName;
    public TMP_Text NotepadMenuEditName;
    public TMP_Text NotepadContent;
    // NoteCopyButton handled in its

    //TravelMenu
    public TMP_Text TravelMenuTopName;
    public TMP_Text TravelMenuName;

    //SettingsMenu
    public TMP_Text SettingsMenuTopName;
    public TMP_Text SettingsMenuTitle;
    public TMP_Text SettingsMenuGlobalOpacity;
    public TMP_Text SettingsMenuBgOpacity;
    public TMP_Text SettingsMenuResolution;
    public TMP_Text SettingsMenuWidth;
    public TMP_Text SettingsMenuHeight;
    public TMP_Text SettingsMenuCopyTravel;
    public TMP_Text SettingsMenuShowGuides;
    public TMP_Text SettingsMenuLanguage;

    public Trad traductor;

    void Awake()
    {
        string tradPath = $"{Application.dataPath}/Scripts/trad.json";
        string tradContent = File.ReadAllText(tradPath);
        traductor = JsonUtility.FromJson<Trad>(tradContent);
        int lang = PlayerPrefs.GetInt("lang", 0);
        string languageCode = lang switch
        {
            0 => "fr",
            1 => "en",
            2 => "es",
            3 => "po",
            _ => "fr",
        };

        MainMenuTopName.text = GetTranslationField(traductor.MainMenu.MenuName, languageCode);
        MainMenuName.text = GetTranslationField(traductor.MainMenu.MenuDesc, languageCode);
        MainMenuPresentation.text = GetTranslationField(traductor.MainMenu.Presentation, languageCode);
        MainMenuDiscordButton.text = GetTranslationField(traductor.MainMenu.DiscordButton, languageCode);
        MainMenuAffiliation.text = GetTranslationField(traductor.MainMenu.Affiliation, languageCode);

        GuidesMenuTopName.text = GetTranslationField(traductor.Guides, languageCode);
        GuidesMenuSearchbar.text = GetTranslationField(traductor.Searchbar, languageCode);

        DownloadMenuTopName.text = GetTranslationField(traductor.DownloadMenu.MenuTitle, languageCode);
        DownloadMenuRootName.text = GetTranslationField(traductor.DownloadMenu.RootMenuTitle, languageCode);
        DownloadMenuDlName.text = GetTranslationField(traductor.Guides, languageCode);
        DownloadMenuCertifiedGuides.text = GetTranslationField(traductor.DownloadMenu.CertifiedGuides, languageCode);
        DownloadMenuPublicGuides.text = GetTranslationField(traductor.DownloadMenu.PublicGuides, languageCode);
        DownloadMenuDraftGuides.text = GetTranslationField(traductor.DownloadMenu.DraftGuides, languageCode);
        DownloadMenuSearchbar.text = GetTranslationField(traductor.Searchbar, languageCode);

        NotepadMenuTopName.text = GetTranslationField(traductor.NotepadMenu.MenuTitle, languageCode);
        NotepadMenuName.text = GetTranslationField(traductor.NotepadMenu.MenuName, languageCode);
        NotepadMenuEditName.text = GetTranslationField(traductor.NotepadMenu.NoteEditionName, languageCode);
        NotepadContent.text = GetTranslationField(traductor.NotepadMenu.NoteContent, languageCode);

        TravelMenuTopName.text = GetTranslationField(traductor.TravelMenu.MenuTitle, languageCode);
        TravelMenuName.text = GetTranslationField(traductor.TravelMenu.MenuTitle, languageCode);

        SettingsMenuTopName.text = GetTranslationField(traductor.SettingsMenu.MenuTitle, languageCode);
        SettingsMenuTitle.text = GetTranslationField(traductor.SettingsMenu.MenuTitle, languageCode);
        SettingsMenuGlobalOpacity.text = GetTranslationField(traductor.SettingsMenu.GlobalOpacity, languageCode);
        SettingsMenuBgOpacity.text = GetTranslationField(traductor.SettingsMenu.BgOpacity, languageCode);
        SettingsMenuResolution.text = GetTranslationField(traductor.SettingsMenu.Resolution, languageCode);
        SettingsMenuWidth.text = GetTranslationField(traductor.SettingsMenu.Width, languageCode);
        SettingsMenuHeight.text = GetTranslationField(traductor.SettingsMenu.Height, languageCode);
        SettingsMenuCopyTravel.text = GetTranslationField(traductor.SettingsMenu.CopyTravel, languageCode);
        SettingsMenuShowGuides.text = GetTranslationField(traductor.SettingsMenu.ShowGuides, languageCode);
        SettingsMenuLanguage.text = GetTranslationField(traductor.SettingsMenu.Language, languageCode);
    }

    public static string GetTranslationField(Trad.TradFormat translation, string languageCode)
    {
        Type type = translation.GetType();
        FieldInfo field = type.GetField(languageCode, BindingFlags.Public | BindingFlags.Instance);
        return field.GetValue(translation) as string;
    }
}
