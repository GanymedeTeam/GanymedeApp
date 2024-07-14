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
using UnityEngine.Assertions.Must;


public class GuideSubstep : MonoBehaviour, IPointerClickHandler
{

    private TMP_Text tmp_text;
    
    Dictionary<string, Regex> bracketPatterns = new Dictionary<string, Regex>()
    {
        { "monster", new Regex(@"<monster dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</monster>") },
        { "object", new Regex(@"<item dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</item>") },
        { "quest", new Regex(@"<quest dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</quest>") },
        { "dungeon", new Regex(@"<dungeon dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</dungeon>") },
    };

    private bool NonConcurrenceFlag = false;

    private Canvas _canvas;
    private Camera _camera;

    private int _targetedSharpColorIndex = -1;
    private string _previousTypeOfColor = "";

    List<(string, string, string)> colors = new List<(string, string, string)>
    {
        {("links", "64F1FF", "7ABBFF")},
        {("pos", "E9FF56", "BDD900")}
    };

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
            {
                string text = tmp_text.textInfo.linkInfo[index].GetLinkID();
                if (text.Contains("http://") || text.Contains("https://"))
                    Application.OpenURL(text);
                if (Regex.Matches(text, @"\[(.*?),(.*?)\]").Count > 0)
                    GUIUtility.systemCopyBuffer = (Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1)) ? "/travel " : "") + text;
            }
        }
    }

    public void ParseCustomBrackets()
    {
        // prevent user from adding links manually
        if (tmp_text.text.Contains("</link>"))
        {
            tmp_text.text = "<color=\"red\">Suspect link detected in step</color>";
            return;
        }
        

        Regex posRegex = new Regex(@"\[(.*?),(.*?)\]");
        MatchCollection matches = posRegex.Matches(tmp_text.text);
        IEnumerable<string> uniqueMatches = matches.OfType<Match>().Select(m => m.Value).Distinct();
        
        foreach ( string posMatch in uniqueMatches)
        {
            string textToWrite = "<link=\"" + posMatch + "\"><color=#" + colors[1].Item2 + ">" + posMatch + "</color></link>";
            tmp_text.text = tmp_text.text.Replace(posMatch, textToWrite);
        }

        bool isFirst;
        int endCursor;

        foreach ( string pattern in bracketPatterns.Keys)
        {
            foreach (Match patternMatch in bracketPatterns[pattern].Matches(tmp_text.text))
            {
                isFirst = false;
                if (patternMatch.Index == 0)
                    isFirst = true;
                // Spaces are for image
                string textToWrite = (
                    "    <link=https://dofusdb.fr/fr/database/" 
                    + pattern + "/" 
                    + patternMatch.Groups[1].Value 
                    + "><color=#" + colors[0].Item2 + ">" 
                    + patternMatch.Groups[3].Value + "</color></link>"
                );
                tmp_text.text = tmp_text.text.Replace(patternMatch.Value, textToWrite);
                endCursor = patternMatch.Index + textToWrite.Count();
                StartCoroutine(AddImageFromLink(patternMatch.Groups[2].Value, patternMatch.Groups[3].Value, endCursor, isFirst));
            }
        }
    }

    private IEnumerable<int> AllIndexesOfText(string text, string searchstring)
    {
        int minIndex = text.IndexOf(searchstring);
        while (minIndex != -1)
        {
            yield return minIndex;
            minIndex = text.IndexOf(searchstring, minIndex + searchstring.Length);
        }
    }

    private IEnumerator AddImageFromLink(string imageUrl, string sChar, int endcursor, bool isfirst)
    {
        while (NonConcurrenceFlag == true)
            yield return 0;
        NonConcurrenceFlag = true;
        yield return 0;

        int realTargetIndex = AllIndexesOfText(tmp_text.text.Substring(0, endcursor), sChar).Last();
        int targetIndex = AllIndexesOfText(
            tmp_text.GetParsedText(), sChar).ElementAt(
                AllIndexesOfText(tmp_text.text.Substring(0, endcursor), sChar
            ).Count() - 1
        );
        Vector3 position = tmp_text.textInfo.characterInfo[targetIndex].bottomLeft;
        Vector3[] v = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(v);

        if (Mathf.Abs(transform.InverseTransformPoint(v[0]).x - position.x) < 20 && !isfirst)
        {
            string base_text = tmp_text.text;
            tmp_text.text = tmp_text.text.Insert(realTargetIndex, "    ");

            yield return 0;
            targetIndex += "    ".Count();
            position = tmp_text.textInfo.characterInfo[targetIndex].bottomLeft;

            // If adding 4 whitespace made it return to line, it deserved to return to line
            if (Mathf.Abs(transform.InverseTransformPoint(v[0]).x - position.x) < 20)
            {
                tmp_text.text = base_text.Insert(realTargetIndex, "\n    ");
                targetIndex += 1;
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
            rawImage.GetComponent<RectTransform>().anchoredPosition = position + transform.position + new Vector3(-10f, 6f, 0f);
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
            if (_previousTypeOfColor == "pos")
                ApplyColorToTargetedLink(colors[1].Item2);
            else
                ApplyColorToTargetedLink(colors[0].Item2);
            _targetedSharpColorIndex = -1;
            _previousTypeOfColor = "";
            return;
        }

        int intersectingLink = TMP_TextUtilities.FindIntersectingLink(tmp_text, mousePosition, _camera);

        if (intersectingLink == -1)
        {
            // reset previous targetet link color to unhovered
            if (_previousTypeOfColor == "pos")
                ApplyColorToTargetedLink(colors[1].Item2);
            else
                ApplyColorToTargetedLink(colors[0].Item2);
            _targetedSharpColorIndex = -1;
            _previousTypeOfColor = "";
            return;
        }

        if (_targetedSharpColorIndex == -1)
        {
            // find targeted link to set its color
            TMP_LinkInfo linkInfo = tmp_text.textInfo.linkInfo[intersectingLink];

            _targetedSharpColorIndex = linkInfo.linkIdFirstCharacterIndex;
            if (Regex.Matches(linkInfo.GetLinkText(), @"\[(.*?),(.*?)\]").Count > 0)
                _previousTypeOfColor = "pos";
            else
                _previousTypeOfColor = "links";
            
            try
            {
                while (tmp_text.text[_targetedSharpColorIndex] != '#')
                    _targetedSharpColorIndex++;
            }
            catch
            {
                Debug.Log("Couldn't find link to apply color");
            }


            // set its color to hovered
            if (_previousTypeOfColor == "pos")
                ApplyColorToTargetedLink(colors[1].Item3);
            else
                ApplyColorToTargetedLink(colors[0].Item3);
        }
    }

    void Update()
    {
        ColorLinks();
    }
}