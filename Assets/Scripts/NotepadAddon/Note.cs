using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public int noteId;
    public string noteName;
    public string noteContent;

    public GameObject Notepad;
    public TMP_Text CopyButtonText;

    void Awake()
    {
        int lang = PlayerPrefs.GetInt("lang", 0);
        switch (lang)
        {
            case 0: //fr
                CopyButtonText.text = "Copier";
                break;

            case 1: //en
                CopyButtonText.text = "Copy";
                break;

            case 2: //es
                CopyButtonText.text = "Copia";
                break;

            case 3: //po
                CopyButtonText.text = "Cópia";
                break;
        }
    }

    public void OnClickModifyCurrentID()
    {
        Notepad.GetComponent<Notepad>().targetedNote = this;
    }

    public void CopyContent()
    {
        GUIUtility.systemCopyBuffer = noteContent;
    }
}
