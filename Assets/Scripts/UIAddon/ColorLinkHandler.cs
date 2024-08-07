using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;

public class ColorLinkHandler : MonoBehaviour
{
    private int _targetedSharpColorIndex = -1;
    private int idIntersectedLink = -1;
    private TMP_Text tmp_text;

    private Canvas _canvas;
    private Camera _camera;

    public struct ColorPair
    {
        public string HoverColor { get; set; }
        public string UnhoverColor { get; set; }

        public ColorPair(string hoverColor, string unhoverColor)
        {
            HoverColor = hoverColor;
            UnhoverColor = unhoverColor;
        }
    }

    public Dictionary<string, ColorPair> ColorDictionary = new Dictionary<string, ColorPair>
    {
        { "classic_link", new ColorPair("#cfdcfa", "#aac2fa")},
        { "quest", new ColorPair("#7ede9d", "#2cc75d")},
        { "object", new ColorPair("#f7cd9c", "#db9542")},
        { "monster", new ColorPair("#e38485", "#d94a4b")},
        { "dungeon", new ColorPair("#f0a8dd", "#eb5bc6")},
        { "pos", new ColorPair("#fffec2", "#FFFD01")},
        { "gotoguide", new ColorPair("#c7bce6", "#8e7cc3")},
    };

    void Start()
    {
        tmp_text = transform.GetComponent<TMP_Text>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            _camera = null;
        else
            _camera = _canvas.worldCamera;
    }

    public void ColorLinks()
    {
        void SetColor(string mode)
        {
            
            if (_targetedSharpColorIndex != -1)
            {
                string prevColor = tmp_text.text.Substring(_targetedSharpColorIndex, 7);
                string newColor = "";

                if (mode == "unhover")
                {
                    foreach ( ColorPair cp in ColorDictionary.Values)
                    {
                        if (cp.HoverColor == prevColor)
                        {
                            newColor = cp.UnhoverColor;
                            break;
                        }
                    }
                }

                else if (mode == "hover")
                {
                    foreach ( ColorPair cp in ColorDictionary.Values)
                    {
                        if (cp.UnhoverColor == prevColor)
                        {
                            newColor = cp.HoverColor;
                            break;
                        }
                    }
                }

                if (newColor == "")
                    return;

                tmp_text.text = tmp_text.text.Remove(_targetedSharpColorIndex, 7).Insert(_targetedSharpColorIndex, newColor);
            }
        }

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

        bool isIntersectingRectTransform = TMP_TextUtilities.IsIntersectingRectTransform(tmp_text.GetComponent<RectTransform>(), mousePosition, _camera);

        if (!isIntersectingRectTransform)
        {
            // reset previous targetet link color to unhovered
            SetColor("unhover");
            _targetedSharpColorIndex = -1;
            idIntersectedLink = -1;
            return;
        }

        int intersectingLink = TMP_TextUtilities.FindIntersectingLink(tmp_text, mousePosition, _camera);

        if (intersectingLink == -1)
        {
            // reset previous targetet link color to unhovered
            SetColor("unhover");
            _targetedSharpColorIndex = -1;
            idIntersectedLink = -1;
            return;
        }

        if (idIntersectedLink != -1 && idIntersectedLink != tmp_text.textInfo.linkInfo[intersectingLink].linkIdFirstCharacterIndex)
        {
            // reset previous targetet link color to unhovered
            SetColor("unhover");
            _targetedSharpColorIndex = -1;
            idIntersectedLink = -1;
            return;
        }

        if (_targetedSharpColorIndex == -1)
        {
            // find targeted link to set its color
            TMP_LinkInfo linkInfo = tmp_text.textInfo.linkInfo[intersectingLink];

            _targetedSharpColorIndex = linkInfo.linkIdFirstCharacterIndex;
            
            try
            {
                while (tmp_text.text.Substring(_targetedSharpColorIndex, 8) != "<color=#")
                    _targetedSharpColorIndex++;
                _targetedSharpColorIndex += 7;
            }
            catch
            {
                Debug.Log("Couldn't find link to apply color");
            }

            SetColor("hover");
            idIntersectedLink = linkInfo.linkIdFirstCharacterIndex;
        }
    }

    void Update()
    {
        ColorLinks();
    }
}