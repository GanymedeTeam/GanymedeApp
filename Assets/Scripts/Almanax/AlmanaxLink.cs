using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Assertions.Must;

public class AlmanaxLink : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text tmp_text;

    void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int index = TMP_TextUtilities.FindIntersectingLink(tmp_text, Input.mousePosition, null);
            if (index > -1)
            {
                string text = tmp_text.textInfo.linkInfo[index].GetLinkID();
                if (text.Contains("http://") || text.Contains("https://"))
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        Application.OpenURL(text);
                    }
                    else
                    {
                        GUIUtility.systemCopyBuffer = tmp_text.textInfo.linkInfo[index].GetLinkText();
                    }
                }
            }
        }
    }
}
