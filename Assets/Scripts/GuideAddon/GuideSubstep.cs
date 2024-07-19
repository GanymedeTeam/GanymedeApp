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


public class GuideSubstep : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text tmp_text;
    List<int> linkPositions = new List<int>();

    List<bool> stateOfCoroutines;
    int nbOfCustomLinks = -1;

    readonly List<string> linksWhitelist = new List<string>
    {
        "https://www.dofuspourlesnoobs.com/",
        "https://huzounet.fr/"
    };

    // monster : @"<monster dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</monster>")
    // object : @"<item dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</item>"
    // quest : @"<quest dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</quest>"
    // dungeon : @"<dungeon dofusdb=""(\d+)"" imageurl=""(.+?)"">(.+?)</dungeon>"
    string sPattern = @"<(\w+) dofusdb=""(\d+)"" imageurl=""([^""]+)"">([^<]+)<\/\1>";

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
        var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
        return $"<link=\"{coordinate}\"><color={cd["pos"].UnhoverColor}>{coordinate}</color></link>";
    }

    public void ParseCustomBrackets()
    {
        // prevent user from adding links manually
        Regex linkRegex = new Regex(@"<link=""([^""]+)"">([^<]+)<\/link>");
        while (linkRegex.Matches(tmp_text.text).Count != 0)
        {
            string textToWrite;
            if (linksWhitelist.Any(url => linkRegex.Matches(tmp_text.text)[0].Groups[1].Value.StartsWith(url)))
            {
                Match m = linkRegex.Matches(tmp_text.text)[0];
                var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
                textToWrite = $"<link={m.Groups[1].Value}><color={cd["classic_link"].UnhoverColor}>{m.Groups[2].Value}</color></link>";
                tmp_text.text = tmp_text.text.Replace(linkRegex.Matches(tmp_text.text)[0].Value, textToWrite);
            }
            else
            {
                textToWrite = "<color=\"red\">[lien suspect]</color>";
                // Debug.Log(linkRegex.Matches(tmp_text.text)[0].Value);
                tmp_text.text = tmp_text.text.Replace(linkRegex.Matches(tmp_text.text)[0].Value, textToWrite);
                tmp_text.ForceMeshUpdate();
            }

        }
 
        Regex coordRegex = new Regex(@"\[(-?\d+),(-?\d+)\]");
        tmp_text.text = coordRegex.Replace(tmp_text.text, new MatchEvaluator(ReplaceCoordinates));
        tmp_text.ForceMeshUpdate();
        ParseCustomLinks();
    }

    private IEnumerator DownloadImageAndSetSprite(int id, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            GameObject sprite = new GameObject($"CustomLinkSprite_{id}");
            RawImage rawImage = sprite.AddComponent(typeof(RawImage)) as RawImage;
            rawImage.texture = texture;
            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            rawImage.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
            rawImage.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
            rawImage.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            LayoutElement layoutElement = sprite.AddComponent(typeof(LayoutElement)) as LayoutElement;
            layoutElement.ignoreLayout = true;

            sprite.transform.SetParent(transform.parent);
        }
        stateOfCoroutines[id] = true;
    }

    private void SetImagesPosition()
    {
        if (stateOfCoroutines != null)
        {
            if (stateOfCoroutines.Count == nbOfCustomLinks)
            {
                foreach( Transform customLinkSprite in transform.parent)
                {
                    if (!customLinkSprite.name.Contains("CustomLinkSprite"))
                        continue;
                    RawImage rawImage = customLinkSprite.GetComponent<RawImage>();
                    int indexOfCharacter = linkPositions[int.Parse(customLinkSprite.name.Split('_').Last())];
                    Vector3 position = tmp_text.textInfo.characterInfo[indexOfCharacter].bottomLeft;
                    Vector3 worldPosition = tmp_text.rectTransform.TransformPoint(position);
                    rawImage.GetComponent<RectTransform>().position = worldPosition + new Vector3(10f, 6f, 0f);;
                }
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

    private void ParseCustomLinks()
    {
        StringBuilder output = new StringBuilder(tmp_text.text);

        string parsedText = tmp_text.GetParsedText();

        int offset = 0; // Pour suivre le décalage causé par les remplacements

        // Utilisation d'un HashSet pour éviter de remplacer plusieurs fois le même indice
        HashSet<int> replacedIndices = new HashSet<int>();

        // Ajouter une tabulation au début du texte
        void AddLeadingTab(int startIndex)
        {
            output.Insert(startIndex, "\u00A0\u00A0\u00A0\u00A0");
            offset += 4; // Ajouter 4 pour compenser les espaces
        }

        int imageId = 0;

        MatchCollection sPatternMatches = Regex.Matches(tmp_text.text, sPattern);
        stateOfCoroutines = new List<bool>();
        nbOfCustomLinks = sPatternMatches.Count;

        foreach (Match match in sPatternMatches)
        {
            int matchStartIndex = match.Index + offset;

            // Vérifier si cet indice a déjà été remplacé
            if (replacedIndices.Contains(matchStartIndex))
                continue;

            stateOfCoroutines.Add(false);
            StartCoroutine(DownloadImageAndSetSprite(imageId++, match.Groups[3].Value));
            string patternName = match.Groups[1].Value.Replace("item", "object");
            string dofusdbId = match.Groups[2].Value;
            string url = $"https://dofusdb.fr/fr/database/{patternName}/{dofusdbId}";
            string name = match.Groups[4].Value;
            var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
            string replacement = $"<link={url}><color={cd[patternName].UnhoverColor}>{name}</color></link>";

            output.Remove(matchStartIndex, match.Length);
            output.Insert(matchStartIndex, replacement);

            // Calcul des indices dans le nouveau texte et dans le ParsedText
            int newMatchIndex = matchStartIndex + replacement.Length - 1; // -1 pour pointer à la fin du remplacement
            int parsedTextIndex = parsedText.IndexOf(match.Value);

            // matchesInfo.Add((match.Value, newMatchIndex, parsedTextIndex));

            // Marquer cet indice comme remplacé
            replacedIndices.Add(matchStartIndex);

            // Mise à jour de l'offset
            offset += replacement.Length - match.Length;

            // Ajouter une tabulation au début du match
            AddLeadingTab(matchStartIndex);
        }

        tmp_text.text = output.ToString();
        tmp_text.ForceMeshUpdate();

        // Recuperer les index des links pour construire les images
        linkPositions = new List<int>();
        Dictionary<string, int> occurrenceCount = new Dictionary<string, int>();
        foreach (Match match in sPatternMatches)
        {
            string linkTxt = match.Groups[4].Value;
            if (occurrenceCount.ContainsKey(linkTxt))
            {
                occurrenceCount[linkTxt]++;
            }
            else
            {
                occurrenceCount[linkTxt] = 1;
            }
            int index = AllIndexesOfText(tmp_text.GetParsedText(), $"\u00A0\u00A0\u00A0\u00A0{linkTxt}").ElementAt(occurrenceCount[linkTxt] - 1);
            linkPositions.Add(index);
        }
    }

    void Update()
    {
        SetImagesPosition();
    }
}