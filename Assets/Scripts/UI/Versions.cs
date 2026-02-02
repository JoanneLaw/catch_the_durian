using TMPro;
using UnityEngine;

public class Versions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText = null;
    [SerializeField] private GameVersions gameVersions = null;

    void Awake()
    {
        string versionCode = "";

#if GAME_DEBUG_MODE
        #if UNITY_ANDROID
            versionCode = $" ({gameVersions.androidVersionCode})";
        #elif UNITY_IOS
            versionCode = $" ({gameVersions.IosVersionCode})";
        #endif
#endif
        versionText.SetText(Application.version + versionCode);
    }
}
