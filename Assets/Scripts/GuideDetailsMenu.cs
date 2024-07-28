using UnityEngine;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject guideMenu;

    void OnEnable()
    {
        guideMenu.GetComponent<GuideMenu>().LoadGuide(guideMenu.GetComponent<GuideMenu>().OpenedGuide);
    }
}
