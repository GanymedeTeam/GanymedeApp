using UnityEngine;
using TMPro;

public class ResetTMPInputFieldPosition : MonoBehaviour
{
    private TMP_InputField tmpInputField;

    void Awake()
    {
        tmpInputField = GetComponent<TMP_InputField>();
    }

    public void ResetPosition()
    {
        // Reset the "Left" and "Right" of the RectTransform of the Text Area/Text
        RectTransform textRectTransform = tmpInputField.textComponent.rectTransform;
        textRectTransform.offsetMin = new Vector2(0, textRectTransform.offsetMin.y); // Reset left offset
        textRectTransform.offsetMax = new Vector2(0, textRectTransform.offsetMax.y); // Reset right offset

        // Reset the "Left" and "Right" of the RectTransform of the Text Area/Caret
        RectTransform caretRectTransform = tmpInputField.transform.Find("Text Area/Caret").GetComponent<RectTransform>();
        caretRectTransform.offsetMin = new Vector2(0, caretRectTransform.offsetMin.y); // Reset left offset
        caretRectTransform.offsetMax = new Vector2(0, caretRectTransform.offsetMax.y); // Reset right offset
    }
}
