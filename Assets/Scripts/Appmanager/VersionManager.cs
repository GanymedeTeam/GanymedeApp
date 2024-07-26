using UnityEngine;
using TMPro;

public class VersionManager : MonoBehaviour
{
    public TMP_Text version;

    void Awake()
    {
        version.text = $"v{Application.version}";
    }
}
