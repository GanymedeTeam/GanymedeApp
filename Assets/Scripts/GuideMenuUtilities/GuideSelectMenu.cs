using System.Collections;
using UnityEngine;

public class GuideSelectMenu : MonoBehaviour
{
    private GameObject guideMenu;

    void Awake()
    {
        guideMenu = transform.parent.gameObject;
    }

    void OnEnable()
    {
        guideMenu.GetComponent<GuideMenu>().OnClickReloadGuideList();
    }
}