using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GuideSubstepImage : MonoBehaviour
{

    private RawImage rawImage;
    private AspectRatioFitter ratioFitter;

    // readonly List<string> linksWhitelist = new List<string>
    // {
    //     "https://www.dofuspourlesnoobs.com/",
    //     "https://huzounet.fr/",
    //     "https://static.ankama.com/"
    // };

    void Awake()
    {
        rawImage = transform.GetChild(0).GetComponent<RawImage>();
        ratioFitter = transform.GetChild(0).GetComponent<AspectRatioFitter>();
    }

    public void SetImageUrl(string url, float ratio)
    {
        StartCoroutine(LoadImageFromUrl(url, ratio));
    }

    private IEnumerator LoadImageFromUrl(string url, float ratio)
    {
        // if (!linksWhitelist.Any(prefix => url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        //     yield break;

        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            ratioFitter.aspectRatio = (float)texture.width / texture.height;

            float parentWidth = GetComponent<RectTransform>().rect.width;
            float newParentHeight = parentWidth / ratioFitter.aspectRatio / (2.5f - ratio);
            if (ratioFitter.aspectRatio <= 1)
                newParentHeight *= 0.6f;
            else if (ratioFitter.aspectRatio <= 2)
                newParentHeight *= 0.8f;
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newParentHeight);

            rawImage.texture = texture;
        }
        else
        {
            Debug.LogError($"Failed to load image from URL: {url}, Error: {www.error}");
        }
    }
}
