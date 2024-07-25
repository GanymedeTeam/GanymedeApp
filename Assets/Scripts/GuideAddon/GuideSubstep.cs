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
    public int guideId;
    private TMP_Text tmp_text;
    List<int> linkPositions = new List<int>();

    List<bool> stateOfCoroutines;
    int nbOfCustomLinks = -1;

    readonly List<string> linksWhitelist = new List<string>
    {
        "https://www.dofuspourlesnoobs.com/",
        "https://huzounet.fr/"
    };

    readonly string dofusdbLinksPattern = @"<(\w+) dofusdb=""(\d+)"" imageurl=""([^""]+)"">([^<]+)<\/\1>";
    readonly string selfGuideLinksPattern = @"<guide id=""(\d+)"" step=""(\d+)"">([^<]+)<\/guide>";

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
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        GUIUtility.systemCopyBuffer = tmp_text.textInfo.linkInfo[index].GetLinkText();
                    }
                    else
                    {
                        Application.OpenURL(text);
                    }
                }
                if (Regex.Matches(text, @"\[(.*?),(.*?)\]").Count > 0)
                {
                    GUIUtility.systemCopyBuffer = (Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1)) ? "/travel " : "") + text;
                }
                if (Regex.Matches(text, @"guide_(\d+)_step_(\d+)").Count > 0)
                {
                    StartCoroutine(ChangeGuide(
                        int.Parse(Regex.Match(text, @"guide_(\d+)_step_(\d+)").Groups[1].Value),
                        int.Parse(Regex.Match(text, @"guide_(\d+)_step_(\d+)").Groups[2].Value)
                    ));
                }
            }
        }
    }

    private IEnumerator ChangeGuide(int id, int step)
    {
        if (guideId == id)
        {
            FindObjectOfType<GuideMenu>().GoToGuideStep(step - 1);
            yield break;
        }
        string[] listOfIdGuides = Directory.GetFiles(Application.persistentDataPath + "/guides/", $"{id}.json", SearchOption.AllDirectories);
        if (listOfIdGuides.Count() > 0)
        {
            // Guide is downloaded and is somewhere in path
            string path = listOfIdGuides[0];

            // We need to update it
            using UnityWebRequest webRequest = UnityWebRequest.Get($"{Constants.ganymedeWebGuidesUrl}/{id}");
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                System.IO.File.WriteAllText(path, jsonResponse);
            }
        }
        else
        {
            // It is not downloaded yet
            // Download it and place it in root folder
            using UnityWebRequest webRequest = UnityWebRequest.Get($"{Constants.ganymedeWebGuidesUrl}/{id}");
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                string path = Application.persistentDataPath + "/guides";
                System.IO.File.WriteAllText($"{path}/{id}.json", jsonResponse);
            }
        }
        if (step != 0)
        {
            // We set the step where we want to go
            PlayerPrefs.SetInt($"{id}_currstep", step - 1);
        }
        // We open it
        FindObjectOfType<GuideMenu>().LoadGuide(id.ToString());
    }

    string ReplaceCoordinates(Match match)
    {
        string x = match.Groups[1].Value;
        string y = match.Groups[2].Value;
        string coordinate = $"[{x},{y}]";
        var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
        return $"<link=\"{coordinate}\"><color={cd["pos"].UnhoverColor}>{coordinate}</color></link>";
    }

    string ReplaceGoToGuides(Match match)
    {
        string id = match.Groups[1].Value;
        string step = match.Groups[2].Value;
        string guideName = match.Groups[3].Value;
        string gotoGuide = $"guide_{id}_step_{step}";
        var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
        return $"<link=\"{gotoGuide}\"><color={cd["gotoguide"].UnhoverColor}>{guideName}</color></link>";
    }

    private void ParseNoLogoObjects()
    {
        Regex coordRegex = new Regex(@"\[(-?\d+),(-?\d+)\]");
        tmp_text.text = coordRegex.Replace(tmp_text.text, new MatchEvaluator(ReplaceCoordinates));
        tmp_text.ForceMeshUpdate();
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
                tmp_text.text = tmp_text.text.Replace(linkRegex.Matches(tmp_text.text)[0].Value, textToWrite);
                tmp_text.ForceMeshUpdate();
            }

        }

        ParseNoLogoObjects();
        ParseLogoObjects();
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
            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);
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
                    try
                    {
                        if (!customLinkSprite.name.Contains("CustomLinkSprite"))
                            continue;
                        RawImage rawImage = customLinkSprite.GetComponent<RawImage>();
                        int indexOfCharacter = linkPositions[int.Parse(customLinkSprite.name.Split('_').Last())];
                        Vector3 position = tmp_text.textInfo.characterInfo[indexOfCharacter].bottomLeft;
                        Vector3 worldPosition = tmp_text.rectTransform.TransformPoint(position);
                        rawImage.GetComponent<RectTransform>().position = worldPosition + new Vector3(8.5f, 6f, 0f);
                    }
                    catch{};
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

    private IEnumerable<int> AllIndexesOfExactText(string text, string searchString)
    {
        int startIndex = 0;
        while (true)
        {
            // Cherche la prochaine occurrence de searchString à partir de startIndex
            int minIndex = text.IndexOf(searchString, startIndex);
            
            // Si aucune occurrence n'est trouvée, quitter la boucle
            if (minIndex == -1)
                yield break;

            // Vérifie si l'occurrence trouvée est exactement ce que nous recherchons
            if (text.Substring(minIndex, searchString.Length) == searchString)
            {
                yield return minIndex;
            }

            // Déplace le startIndex pour chercher la prochaine occurrence après l'actuelle
            startIndex = minIndex + searchString.Length;
        }
    }

    private void ParseLogoObjects()
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

        MatchCollection dofusdbLinksPatternMatches = Regex.Matches(tmp_text.text, dofusdbLinksPattern);
        MatchCollection selfGuideLinksPatternMatches = Regex.Matches(tmp_text.text, selfGuideLinksPattern);
        var combinedMatches = dofusdbLinksPatternMatches.Cast<Match>()
                                .Concat(selfGuideLinksPatternMatches.Cast<Match>())
                                .OrderBy(m => m.Index)
                                .ToList();

        stateOfCoroutines = new List<bool>();
        nbOfCustomLinks = combinedMatches.Count;

        foreach (Match match in combinedMatches)
        {
            int matchStartIndex = match.Index + offset;

            // Vérifier si cet indice a déjà été remplacé
            if (replacedIndices.Contains(matchStartIndex))
                continue;

            stateOfCoroutines.Add(false);
            string patternName = match.Groups[1].Value.Replace("item", "object");
            var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
            string replacement = "";
            if (match.Value.StartsWith("<guide"))
            {
                StartCoroutine(DownloadImageAndSetSprite(imageId++, "https://api.dofusdb.fr/img/items/1000/25010.png"));
                string id = match.Groups[1].Value;
                string step = match.Groups[2].Value;
                string name = match.Groups[3].Value;
                string url = $"guide_{id}_step_{step}";
                replacement = $"<link={url}><color={cd["gotoguide"].UnhoverColor}>{name}</color></link>";
            }
            else
            {
                StartCoroutine(DownloadImageAndSetSprite(imageId++, match.Groups[3].Value));
                string dofusdbId = match.Groups[2].Value;
                string url = $"https://dofusdb.fr/fr/database/{patternName}/{dofusdbId}";
                string name = match.Groups[4].Value;
                replacement = $"<link={url}><color={cd[patternName].UnhoverColor}>{name}</color></link>";
            }

            output.Remove(matchStartIndex, match.Length);
            output.Insert(matchStartIndex, replacement);

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
        List<string> matchesSeen = new List<string>();
        foreach (Match match in combinedMatches)
        {
            string linkTxt;
            int indexOffset = 0;
            if (match.Value.StartsWith("<guide"))
                linkTxt = match.Groups[3].Value;
            else
                linkTxt = match.Groups[4].Value;
            foreach (string previousMatch in matchesSeen)
            {
                if (previousMatch.Contains(linkTxt))
                    indexOffset++;
            }
            int index = AllIndexesOfExactText(tmp_text.GetParsedText(), $"\u00A0\u00A0\u00A0\u00A0{linkTxt}").ElementAt(indexOffset);
            linkPositions.Add(index);
            matchesSeen.Add(linkTxt);
        }
    }

    void Update()
    {
        SetImagesPosition();
    }
}