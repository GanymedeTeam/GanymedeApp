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
using UnityEngine.Animations;


public class GuideSubstep : MonoBehaviour, IPointerClickHandler
{
    public int guideId;
    private TMP_Text tmp_text;
    private List<int> linkPositions = new List<int>();

    private List<bool> stateOfCoroutines;
    private int nbOfCustomLinks = -1;

    private List<string> storedUrls = new List<string>();

    readonly List<string> linksWhitelist = new List<string>
    {
        "https://www.dofuspourlesnoobs.com/",
        "https://huzounet.fr/",
        "https://www.dofusbook.net/",
        "https://ganymede-dofus.com/",
        "https://dofus-portals.fr/",
        "https://www.youtube.com/",
        "https://twitter.com/",
        "https://x.com/",
        "https://www.dofus.com/",
        "https://www.twitch.tv/",
        "https://metamob.fr/",
        "https://dofusdb.fr/",
        "https://www.dofuskin.com/",
        "https://barbofus.com/",
        "https://dofensive.com/",
        "https://www.dofuskin.com/",  
    };

    readonly string dofusdbLinksPattern = @"<(\w+) dofusdb=""(\d+)"" imageurl=""([^""]+)"">([^<]+)<\/\1>";
    readonly string selfGuideLinksPattern = @"<guide id=""(\d+)"" step=""(\d+)"">([^<]+)<\/guide>";

    void Start()
    {
        tmp_text = transform.GetComponent<TMP_Text>();
        tmp_text.fontSize = PlayerPrefs.GetInt("fontSize", 14);
        transform.parent.GetComponent<HorizontalLayoutGroup>().spacing = 18.76f + Mathf.Abs(tmp_text.fontSize - 14)*0.5f;
        transform.parent.Find("Toggle/Background").GetComponent<RectTransform>().sizeDelta =
        new Vector2(18f + Mathf.Abs(tmp_text.fontSize - 14) * 0.5f, 18f + Mathf.Abs(tmp_text.fontSize - 14) * 0.5f);
        transform.parent.Find("Toggle/Background").GetComponent<RectTransform>().anchoredPosition =
        new Vector2(18f + Mathf.Abs(tmp_text.fontSize - 14) * 0.5f, -(18f + Mathf.Abs(tmp_text.fontSize - 14) * 1.2f))/2;
        ParseCustomBrackets();
        if (tmp_text.text.EndsWith("<br>"))
            tmp_text.text += "<br>";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int index = TMP_TextUtilities.FindIntersectingLink(tmp_text, Input.mousePosition, null);
            if (index > -1)
            {
                string text = tmp_text.textInfo.linkInfo[index].GetLinkID();
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Application.OpenURL(storedUrls[int.Parse(text)]);
                }
                else
                {
                    GUIUtility.systemCopyBuffer = tmp_text.textInfo.linkInfo[index].GetLinkText();
                }
                if (Regex.Matches(text, @"\[(.*?),(.*?)\]").Count > 0)
                {
                    Match pos = Regex.Match(text, @"\[(.*?),(.*?)\]");
                    bool wantTravel = Convert.ToBoolean(PlayerPrefs.GetInt("wantTravel", 1));
                    string pos_x = pos.Groups[1].Value;
                    string pos_y = pos.Groups[2].Value;
                    if (wantTravel)
                        GUIUtility.systemCopyBuffer = $"/travel {pos_x},{pos_y}";
                    else
                        GUIUtility.systemCopyBuffer = $"[{pos_x},{pos_y}]";
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
            FindObjectOfType<GuideMenu>().GoToGuideStep(step);
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
                GuideEntry guide = JsonUtility.FromJson<GuideEntry>(jsonResponse);
                string path = Application.persistentDataPath + "/guides";
                if (guide.status == "gp")
                {
                    if (!Directory.Exists($"{Application.persistentDataPath}/guides/GP"))
                        Directory.CreateDirectory($"{Application.persistentDataPath}/guides/GP");
                    path += "/GP";
                }
                System.IO.File.WriteAllText($"{path}/{id}.json", jsonResponse);
            }
        }
        // We open it
        if (step == 0)
        {
            // Load save, else go step 1
            try
            {
                step = FindObjectOfType<SaveManager>().saveProgress.guideProgress.Find(e => e.id == id).current_step;
            }
            catch
            {
                step = 1;
            }
        }
        FindObjectOfType<GuideMenu>().LoadGuide(id.ToString());
        FindObjectOfType<GuideMenu>().GoToGuideStep(step);

    }

    string ReplaceCoordinates(Match match)
    {
        string x = match.Groups[1].Value;
        string y = match.Groups[2].Value;
        string coordinate = $"[{x},{y}]";
        var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;
        return $"<link=\"{coordinate}\"><color={cd["pos"].UnhoverColor}>{coordinate}</color></link>";
    }

    private void ParseNoLogoObjects()
    {
        Regex coordRegex = new Regex(@"\[(-?\d+),(-?\d+)\]");
        tmp_text.text = coordRegex.Replace(tmp_text.text, new MatchEvaluator(ReplaceCoordinates));
        tmp_text.ForceMeshUpdate();
    }

    public void ParseCustomBrackets()
    {
        Regex linkRegex = new Regex(@"<link=""([^""]+)"">([^<]+)<\/link>");
        var cd = gameObject.GetComponent<ColorLinkHandler>().ColorDictionary;

        //store every link in table and replace it
        foreach (Match match in linkRegex.Matches(tmp_text.text))
        {
            tmp_text.text = tmp_text.text.Replace(match.Value, $"<link={storedUrls.Count}>{match.Groups[2].Value}</link>");
            storedUrls.Add(match.Groups[1].Value);
        }
        // remove unwanted urls to front
        linkRegex = new Regex(@"<link=([^>]+)>([^<]+)<\/link>");
        while (linkRegex.Matches(tmp_text.text).Count != 0)
        {
            string textToWrite;
            int urlIndex = int.Parse(linkRegex.Matches(tmp_text.text)[0].Groups[1].Value);
            if (linksWhitelist.Any(url => storedUrls[urlIndex].StartsWith(url)))
            {
                Match m = linkRegex.Matches(tmp_text.text)[0];
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
            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(tmp_text.fontSize + 1, tmp_text.fontSize + 1);
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
                        rawImage.GetComponent<RectTransform>().position = 
                        worldPosition + new Vector3(8.5f + 0.5f*Mathf.Abs(tmp_text.fontSize - 14), 5f, 0f);
                    }
                    catch{};
                }
            }
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
                StartCoroutine(DownloadImageAndSetSprite(imageId++, "https://ganymede-dofus.com/images/texteditor/guides.png"));
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
                replacement = $"<link={storedUrls.Count}><color={cd[patternName].UnhoverColor}>{name}</color></link>";
                storedUrls.Add(url);
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
            int index;
            try
            {
                index = AllIndexesOfExactText(tmp_text.GetParsedText(), $"\u00A0\u00A0\u00A0\u00A0{linkTxt}").ElementAt(indexOffset);
            }
            catch
            {
                index = AllIndexesOfExactText(tmp_text.GetParsedText(), $"\u00A0\u00A0\u00A0\u00A0{linkTxt}").Last();
            }
            linkPositions.Add(index);
            matchesSeen.Add(linkTxt);
        }
    }

    void Update()
    {
        SetImagesPosition();
    }
}