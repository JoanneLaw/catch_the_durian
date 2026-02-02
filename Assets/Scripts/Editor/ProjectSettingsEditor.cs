using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Build;
using System.Linq;
using System.Collections.Generic;

public class ProjectSettingsEditor : OdinEditorWindow
{
    [MenuItem("Tools/Project Settings Editor")]
    private static void OpenWindow()
    {
        GetWindow<ProjectSettingsEditor>().Show();
    }
    private string _version = "";
    [ShowInInspector, BoxGroup("Game Versions")]
    public string Version
    {
        get
        {
            if (_version == "")
                _version = PlayerSettings.bundleVersion;

            return _version;
        }
        set
        {
            _version = value;
        }
    }
    private int _androidVersionCode = -1;
    [ShowInInspector, BoxGroup("Game Versions")]
    public int AndroidVersionCode
    {
        get
        {
            if (_androidVersionCode < 0)
                _androidVersionCode = PlayerSettings.Android.bundleVersionCode;

            return _androidVersionCode;
        }
        set
        {
            _androidVersionCode = value;
        }
    }
    private int _iosVersionCode = -1;
    [ShowInInspector, BoxGroup("Game Versions"), LabelText("iOS Build Number")]
    public int IosVersionCode
    {
        get
        {
            if (_iosVersionCode < 0)
            {
                if (int.TryParse(PlayerSettings.iOS.buildNumber, out int buildNumber))
                    _iosVersionCode = buildNumber;
            }

            return _iosVersionCode;
        }
        set
        {
            _iosVersionCode = value;
        }
    }
    [Button(ButtonSizes.Large), BoxGroup("Game Versions")]
    public void CopyToSettings()
    {
        PlayerSettings.bundleVersion = _version;
        PlayerSettings.Android.bundleVersionCode = _androidVersionCode;
        PlayerSettings.iOS.buildNumber = _iosVersionCode.ToString();

        if (gameVersions != null)
        {
            gameVersions.androidVersionCode = _androidVersionCode;
            gameVersions.iOSVersionCode = _iosVersionCode;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(gameVersions);
        }
    }
    private GameVersions gameVersions = null;
    private List<string> androidSymbolsList = null;
    private List<string> iosSymbolsList = null;
    private bool isDebugMode = false;
    [ShowInInspector, BoxGroup("Project Settings"), LabelText("Debug Mode")]
    public bool IsDebugMode
    {
        get => isDebugMode;
        set
        {
            if (isDebugMode != value)
            {
                isDebugMode = value;
                UpdateSymbolToPlayerSettings("GAME_DEBUG_MODE", !value);
            }
        }
    }
    private bool isTweenDebug = false;
    [ShowInInspector, BoxGroup("Project Settings"), LabelText("Tween Debug (works only in Development Build)")]
    public bool IsTweenDebug
    {
        get => isTweenDebug;
        set
        {
            if (isTweenDebug != value)
            {
                isTweenDebug = value;
                UpdateSymbolToPlayerSettings("PRIME_TWEEN_SAFETY_CHECKS", !value);
            }
        }
    }
    protected override void OnBeginDrawEditors()
    {
        gameVersions = AssetDatabase.LoadAssetAtPath<GameVersions>("Assets/Scripts/GameVersions/GameVersions.asset");

        string[] androidSymbols, iosSymbols;
        PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android, out androidSymbols);
        PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.iOS, out iosSymbols);
        androidSymbolsList = androidSymbols.ToList();
        iosSymbolsList = iosSymbols.ToList();
        IsDebugMode = androidSymbols.Contains("GAME_DEBUG_MODE") && iosSymbols.Contains("GAME_DEBUG_MODE");
    }

    private void UpdateSymbolToPlayerSettings(string symbol, bool isRemove)
    {
        bool isUpdated = false;

        if (isRemove)
        {
            if (androidSymbolsList.Contains(symbol))
            {
                androidSymbolsList.Remove(symbol);
                isUpdated = true;
            }

            if (iosSymbolsList.Contains(symbol))
            {
                iosSymbolsList.Remove(symbol);
                isUpdated = true;
            }
        }
        else
        {
            if (!androidSymbolsList.Contains(symbol))
            {
                androidSymbolsList.Add(symbol);
                isUpdated = true;
            }

            if (!iosSymbolsList.Contains(symbol))
            {
                iosSymbolsList.Add(symbol);
                isUpdated = true;
            }
        }

        if (isUpdated)
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, androidSymbolsList.ToArray());
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, iosSymbolsList.ToArray());
        }
    }
}
