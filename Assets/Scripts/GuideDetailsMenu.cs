using UnityEngine;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject winManager;
    public GameObject guideMenu;

    void OnEnable()
    {
        guideMenu.GetComponent<GuideMenu>().GoToGuideStep(guideMenu.GetComponent<GuideMenu>().guideProgress);
    }
}
