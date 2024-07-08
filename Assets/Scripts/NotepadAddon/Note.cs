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

    public void OnClickModifyCurrentID()
    {
        Notepad.GetComponent<Notepad>().targetedNote = this;
    }

    public void CopyContent()
    {
        GUIUtility.systemCopyBuffer = noteContent;
    }
}
