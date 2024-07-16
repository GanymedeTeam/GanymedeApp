using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class PaginationHandler : MonoBehaviour
{
    [NonSerialized]
    public const int maxElementsInPage = 5;
    [NonSerialized]
    public int totalElements;
    [NonSerialized]
    public int nbPagesSkip;
    [NonSerialized]
    public int currentPage = 1;
    [NonSerialized]
    public int maxPage;

    public GameObject content;
    public GameObject paginationToolbar;

    void Update()
    {
        if (totalElements < maxElementsInPage)
        {
            maxPage = 1;
            currentPage = 1;
        }
        else if (totalElements % maxElementsInPage == 0)
            maxPage = totalElements / maxElementsInPage;
        else
            maxPage = Mathf.FloorToInt(totalElements / maxElementsInPage) + 1;

        if (totalElements <= maxElementsInPage)
            paginationToolbar.SetActive(false);
        else
            paginationToolbar.SetActive(true);
        paginationToolbar.transform.Find("PageIndication").GetComponent<TMP_Text>().text = currentPage.ToString() + "/" + maxPage;
        if (currentPage == 1)
        {
            try
            {
                paginationToolbar.transform.Find("PreviousPageButton").gameObject.SetActive(false);
                paginationToolbar.transform.Find("FirstPageButton").gameObject.SetActive(false);
            } catch {}
        }
        else
        {
            try
            {
                paginationToolbar.transform.Find("PreviousPageButton").gameObject.SetActive(true);
                paginationToolbar.transform.Find("FirstPageButton").gameObject.SetActive(true);
            } catch {}
        }

        if (currentPage * maxElementsInPage >= totalElements)
        {
            try
            {
                paginationToolbar.transform.Find("NextPageButton").gameObject.SetActive(false);
                paginationToolbar.transform.Find("LastPageButton").gameObject.SetActive(false);
            } catch {}
        }
        else
        {
            try
            {
                paginationToolbar.transform.Find("NextPageButton").gameObject.SetActive(true);
                paginationToolbar.transform.Find("LastPageButton").gameObject.SetActive(true);
            } catch {}
        }
    }

    public void GoNextPage()
    {
        currentPage++;
        TriggerReload();
    }

    public void GoPreviousPage()
    {
        currentPage--;
        TriggerReload();
    }

    public void GoFirstPage()
    {
        currentPage = 1;
        TriggerReload();
    }

    public void GoLastPage()
    {
        currentPage = maxPage;
        TriggerReload();
    }

    void TriggerReload()
    {
        if (gameObject.name == "GuideWindow")
        {
            StartCoroutine(gameObject.GetComponent<GuideMenu>().ReloadGuideList());
        }
        else if (gameObject.name == "DownloadWindow")
        {
            StartCoroutine(gameObject.GetComponent<GuideManager>().GetGuidesList());
        }
    }
}