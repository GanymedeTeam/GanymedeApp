using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;

public class EnterNewLineInputField : MonoBehaviour
{
    TMP_InputField inputfield;
 
    private void Awake()
    {
        inputfield = GetComponent<TMP_InputField>();
    }
 
    IEnumerator FieldFix()
    {
        inputfield.ActivateInputField();
 
        yield return null;
        inputfield.text += "\n";
        inputfield.MoveTextEnd(true);
    }

    void Update () 
    {
        if (EventSystem.current.currentSelectedGameObject == inputfield.gameObject)
        {
            if(Input.GetKeyUp(KeyCode.Return))
            {
                StartCoroutine(FieldFix());
            }
        }
   }
}