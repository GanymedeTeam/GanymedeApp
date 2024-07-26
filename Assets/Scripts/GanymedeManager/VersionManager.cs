using TMPro;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
    public TMP_Text version;

    void Start()
    {
        version.text = $"v{Application.version}";
    }
}
