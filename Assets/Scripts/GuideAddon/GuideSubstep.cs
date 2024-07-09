using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Linq;


public class GuideSubstep : MonoBehaviour, IPointerClickHandler
{

    private TMP_Text tmp_text;

    Regex monsterRegex = new Regex(@"<monster dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</monster>");
    Regex itemRegex = new Regex(@"<item dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</item>");
    Regex questRegex = new Regex(@"<quest dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</quest>");
    Regex dungeonRegex = new Regex(@"<dungeon dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</dungeon>");

    private bool NonConcurrenceFlag = false;

    private Canvas _canvas;
    private Camera _camera;

    private int _targetedSharpColorIndex = -1;

    private const string hoveredLinkColor = "7ABBFF";
    private const string unhoveredLinkColor = "64F1FF";

    void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            _camera = null;
        else
            _camera = _canvas.worldCamera;
        tmp_text = transform.GetComponent<TMP_Text>();
        ParseCustomBrackets();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int index = TMP_TextUtilities.FindIntersectingLink(tmp_text, Input.mousePosition, null);
            if (index > -1)
                Application.OpenURL(tmp_text.textInfo.linkInfo[index].GetLinkID());
        }
    }

    public void ParseCustomBrackets()
    {
        foreach (Match monsterMatch in monsterRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/monster/" 
            + monsterMatch.Groups[1].Value + "><color=#" + unhoveredLinkColor + ">" + monsterMatch.Groups[3].Value + "</color></link>";
            tmp_text.text = tmp_text.text.Replace(monsterMatch.Value, textToWrite);
            StartCoroutine(AddImageFromLink(monsterMatch.Groups[2].Value, monsterMatch.Groups[3].Value));
        }

        foreach (Match itemMatch in itemRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/object/" 
            + itemMatch.Groups[1].Value + "><color=#" + unhoveredLinkColor + ">" + itemMatch.Groups[3].Value + "</color></link>";
            tmp_text.text = tmp_text.text.Replace(itemMatch.Value, textToWrite);
            StartCoroutine(AddImageFromLink(itemMatch.Groups[2].Value, itemMatch.Groups[3].Value));
        }

        foreach (Match questMatch in questRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/quest/" 
            + questMatch.Groups[1].Value + "><color=#" + unhoveredLinkColor + ">" + questMatch.Groups[3].Value + "</color></link>";
            tmp_text.text = tmp_text.text.Replace(questMatch.Value, textToWrite);
            StartCoroutine(AddImageFromLink(questMatch.Groups[2].Value, questMatch.Groups[3].Value));
        }

        foreach (Match dungeonMatch in dungeonRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/dungeon/" 
            + dungeonMatch.Groups[1].Value + "><color=#" + unhoveredLinkColor + ">" + dungeonMatch.Groups[3].Value + "</color></link>";
            tmp_text.text = tmp_text.text.Replace(dungeonMatch.Value, textToWrite);
            StartCoroutine(AddImageFromLink(dungeonMatch.Groups[2].Value, dungeonMatch.Groups[3].Value));

        }
    }

    private IEnumerator AddImageFromLink(string imageUrl, string sChar)
    {
        while (NonConcurrenceFlag == true)
            yield return 0;
        NonConcurrenceFlag = true;
        yield return 0;
        int targetIndex = tmp_text.GetParsedText().IndexOf(sChar);
        int realTargetIndex = tmp_text.text.IndexOf(sChar);
        Vector3 position = tmp_text.textInfo.characterInfo[targetIndex].bottomLeft;
        Vector3[] v = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(v);

        if(Mathf.Abs(transform.InverseTransformPoint(v[0]).x - position.x) < 20)
        {
            string base_text = tmp_text.text;
            tmp_text.text = tmp_text.text.Insert(realTargetIndex, "\t");

            yield return 0;
            targetIndex += "\t".Count();
            position = tmp_text.textInfo.characterInfo[targetIndex].bottomLeft;

            // If adding 4 whitespace made it return to line, it deserved to return to line
            if (Mathf.Abs(transform.InverseTransformPoint(v[0]).x - position.x) < 20)
            {
                tmp_text.text = base_text.Insert(realTargetIndex, "\n\t");
                targetIndex += "\n\t".Count() - 1;
            }
            yield return 0;
            position = tmp_text.textInfo.characterInfo[targetIndex].bottomLeft;
        }
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            GameObject sprite = new GameObject();
            RawImage rawImage = sprite.AddComponent(typeof(RawImage)) as RawImage;
            rawImage.texture = texture;
            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            rawImage.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            rawImage.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            rawImage.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            rawImage.GetComponent<RectTransform>().anchoredPosition = position + transform.position + new Vector3(-9.5f, 5f, 0f);
            sprite.transform.SetParent(tmp_text.transform);
        }
        NonConcurrenceFlag = false;
    }

    public void ColorLinks()
    {
        void ApplyColorToTargetedLink(string hexcolor)
        {
            if (_targetedSharpColorIndex != -1)
            {
                tmp_text.text = tmp_text.text.Remove(_targetedSharpColorIndex + 1, 6).Insert(_targetedSharpColorIndex + 1, hexcolor);
            }
        }

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

        bool isIntersectingRectTransform = TMP_TextUtilities.IsIntersectingRectTransform(tmp_text.GetComponent<RectTransform>(), mousePosition, _camera);

        if (!isIntersectingRectTransform)
        {
            // reset previous targetet link color to unhovered
            ApplyColorToTargetedLink(unhoveredLinkColor);
            _targetedSharpColorIndex = -1;
            return;
        }

        int intersectingLink = TMP_TextUtilities.FindIntersectingLink(tmp_text, mousePosition, _camera);

        if (intersectingLink == -1)
        {
            // reset previous targetet link color to unhovered
            ApplyColorToTargetedLink(unhoveredLinkColor);
            _targetedSharpColorIndex = -1;
            return;
        }

        if (_targetedSharpColorIndex == -1)
        {
            // find targeted link to set its color
            TMP_LinkInfo linkInfo = tmp_text.textInfo.linkInfo[intersectingLink];

            _targetedSharpColorIndex = tmp_text.text.IndexOf(linkInfo.GetLinkText());
            while (tmp_text.text[_targetedSharpColorIndex] != '#')
            {
                if (_targetedSharpColorIndex == 0)
                    break;
                _targetedSharpColorIndex--;
            }

            // set its color to hovered
            ApplyColorToTargetedLink(hoveredLinkColor);
        }
    }

    void Update()
    {
        ColorLinks();
    }
}