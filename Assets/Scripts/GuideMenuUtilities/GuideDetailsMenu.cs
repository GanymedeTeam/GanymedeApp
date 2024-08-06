using System.Collections;
using UnityEngine;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject guideMenu;
    private GuideMenu guideMenuScript;
    public GameObject windowManager;
    private WindowManager windowManagerScript;

    void Awake()
    {
        guideMenuScript = guideMenu.GetComponent<GuideMenu>();
        windowManagerScript = windowManager.GetComponent<WindowManager>();
    }

    void OnEnable()
    {
        StartCoroutine(DelayedRefresh());
    }
    
    IEnumerator DelayedRefresh()
    {
        yield return null; // Wait for one frame
        guideMenuScript.LoadGuide(guideMenu.GetComponent<GuideMenu>().OpenedGuide);
    }
}
