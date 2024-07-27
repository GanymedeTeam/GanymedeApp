using UnityEngine;

public class GuideDetailsMenu : MonoBehaviour
{
    public GameObject winManager;

    void OnEnable()
    {
        winManager.GetComponent<WindowManager>().ToggleInteractiveMap(true);
    }
}
