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

    readonly List<string> linksWhitelist = new List<string>
    {
        "https://www.dofuspourlesnoobs.com/"
    };

    // monster : @"<monster dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</monster>")
    // object : @"<item dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</item>"
    // quest : @"<quest dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</quest>"
    // dungeon : @"<dungeon dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</dungeon>"
    readonly Regex Pattern = new Regex(@"<(\w+) dofusdb=""(\d+)"" imageurl=""([^""]+)"">([^<]+)<\/\1>");

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
        StartCoroutine(ParseCustomBrackets());
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

    string ReplaceCoordinates(Match match)
    {
        string x = match.Groups[1].Value;
        string y = match.Groups[2].Value;
        string coordinate = $"[{x},{y}]";
        // return $"<link=\"{coordinate}\"><color=#12345>{coordinate}</color></link>";
        return $"<link=\"{coordinate}\"><color=#" + colors[1].Item2 + $">{coordinate}</color></link>";

    }

    public IEnumerator ParseCustomBrackets()
    {
        int protection = 0;
        // prevent user from adding links manually
        Regex linkRegex = new Regex(@"<link=""([^""]+)"">([^<]+)<\/link>");
        while (linkRegex.Matches(tmp_text.text).Count != 0 &&
        linksWhitelist.All(url => linkRegex.Matches(tmp_text.text)[0].Value.StartsWith(url)))
        {
            string textToWrite = "<color=\"red\">[lien suspect]</color>";
            tmp_text.text = tmp_text.text.Replace(linkRegex.Matches(tmp_text.text)[0].Value, textToWrite);
            yield return 0;
        }
 
        Regex coordRegex = new Regex(@"\[(-?\d+),(-?\d+)\]");
        tmp_text.text = coordRegex.Replace(tmp_text.text, new MatchEvaluator(ReplaceCoordinates));
        yield return 0;

        bool isFirst;
        int endCursor;
        protection = 0;
        while (Pattern.Matches(tmp_text.text).Count != 0)
        {
            if (protection > 100)
                break;
            protection++;
            Match patternMatch = Pattern.Matches(tmp_text.text)[0];
            string patternName = patternMatch.Groups[1].Value;
            string dofusdbId = patternMatch.Groups[2].Value;
            string imgUrl = patternMatch.Groups[3].Value;
            string sChar = patternMatch.Groups[4].Value;
            isFirst = false;
            if (patternMatch.Index == 0)
                isFirst = true;
            // Spaces are for image
            string textToWrite = (
                "    <link=https://dofusdb.fr/fr/database/" + patternName + "/" + dofusdbId +
                "><color=#" + colors[0].Item2 + ">" + sChar + "</color></link>"
            );
            tmp_text.text = tmp_text.text.Replace(patternMatch.Value, textToWrite);
            endCursor = patternMatch.Index + textToWrite.Length;
            yield return AddImageFromLink(imgUrl, sChar, endCursor, isFirst);
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
        yield return 0;
        char[] k = tmp_text.text.ToCharArray();
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
            rawImage.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
            rawImage.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
            rawImage.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            rawImage.GetComponent<RectTransform>().anchoredPosition = position + transform.position + new Vector3(-10f, 6f, 0f);
            sprite.transform.SetParent(tmp_text.transform);
        }
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