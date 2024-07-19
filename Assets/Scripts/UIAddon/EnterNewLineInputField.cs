using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;

public class EnterNewLineInputField : MonoBehaviour
{
    TMP_InputField inputField;
 
    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
    }
 
    void HandleEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Empêche le comportement par défaut de l'InputField
            EventSystem.current.SetSelectedGameObject(null);

            // Ajoute un retour à la ligne sans désélectionner l'InputField
            inputField.text += "\n";

            // Replace le curseur à la fin du texte
            inputField.caretPosition = inputField.text.Length;

            // Réactive l'InputField
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();
        }
    }
}