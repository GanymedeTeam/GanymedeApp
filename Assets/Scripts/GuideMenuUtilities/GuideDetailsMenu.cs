using UnityEngine;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject guideMenu;
    public GameObject mapManager;
    public GameObject mapMask;
    private GuideMenu guideMenuScript;

    void Start()
    {
        guideMenuScript = guideMenu.GetComponent<GuideMenu>();
    }

    void OnEnable()
    {
        guideMenu.GetComponent<GuideMenu>().LoadGuide(guideMenu.GetComponent<GuideMenu>().OpenedGuide);
    }
}
