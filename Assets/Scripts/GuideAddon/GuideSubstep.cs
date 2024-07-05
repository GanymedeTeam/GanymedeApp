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


public class GuideSubstep : MonoBehaviour, IPointerClickHandler
{

    private TMP_Text tmp_text;

    Regex monsterRegex = new Regex(@"<monster dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</monster>");
    Regex itemRegex = new Regex(@"<item dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</item>");
    Regex questRegex = new Regex(@"<quest dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</quest>");
    Regex dungeonRegex = new Regex(@"<dungeon dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</dungeon>");

    void Start()
    {
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
            + monsterMatch.Groups[1].Value + ">" + monsterMatch.Groups[3].Value + "</link>";
            StartCoroutine(AddImageFromLink(monsterMatch.Groups[2].Value, monsterMatch.Index));
            tmp_text.text = tmp_text.text.Replace(monsterMatch.Value, textToWrite);
        }

        foreach (Match itemMatch in itemRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/object/" 
            + itemMatch.Groups[1].Value + ">" + itemMatch.Groups[3].Value + "</link>";
            StartCoroutine(AddImageFromLink(itemMatch.Groups[2].Value, itemMatch.Index));
            tmp_text.text = tmp_text.text.Replace(itemMatch.Value, textToWrite);
        }

        foreach (Match questMatch in questRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/quest/" 
            + questMatch.Groups[1].Value + ">" + questMatch.Groups[3].Value + "</link>";
            StartCoroutine(AddImageFromLink(questMatch.Groups[2].Value, questMatch.Index));
            tmp_text.text = tmp_text.text.Replace(questMatch.Value, textToWrite);
        }

        foreach (Match dungeonMatch in dungeonRegex.Matches(tmp_text.text))
        {
            // Spaces are for image
            string textToWrite = "    <link=https://dofusdb.fr/fr/database/dungeon/" 
            + dungeonMatch.Groups[1].Value + ">" + dungeonMatch.Groups[3].Value + "</link>";
            StartCoroutine(AddImageFromLink(dungeonMatch.Groups[2].Value, dungeonMatch.Index));
            tmp_text.text = tmp_text.text.Replace(dungeonMatch.Value, textToWrite);
        }
    }

    private IEnumerator AddImageFromLink(string imageUrl, int indexForImage)
    {
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
            rawImage.GetComponent<RectTransform>().anchoredPosition = tmp_text.textInfo.characterInfo[indexForImage].bottomLeft + transform.position + new Vector3(9, 6, 0);
            sprite.transform.SetParent(tmp_text.transform);
        }
    }
}