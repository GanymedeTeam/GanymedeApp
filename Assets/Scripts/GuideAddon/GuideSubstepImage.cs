using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GuideSubstepImage : MonoBehaviour
{
    public RawImage rawImage;
    public float ratio;

    readonly List<string> linksWhitelist = new List<string>
    {
        "https://www.dofuspourlesnoobs.com/",
        "https://huzounet.fr/",
        "https://static.ankama.com/"
    };

    private void Awake()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }
    }

    public void SetImageRatio(float ratio)
    {
        // Set the image ratio (e.g., adjust the RectTransform size)
        if (ratio > 1.5f)
            ratio = 1.5f;
        else if (ratio < 0.5f)
            ratio = 0.5f;
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x) * ratio;
    }

    public void SetImageUrl(string url)
    {
        StartCoroutine(LoadImageFromUrl(url));
    }

    private IEnumerator LoadImageFromUrl(string url)
    {
        if (!linksWhitelist.Any(prefix => url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            yield break;

        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            rawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else
        {
            Debug.LogError($"Failed to load image from URL: {url}, Error: {www.error}");
        }
    }
}
