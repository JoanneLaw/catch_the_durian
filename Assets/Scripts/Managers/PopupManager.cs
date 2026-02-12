using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PopupManager : MonoSingleton<PopupManager>
{
    [Serializable]
    public class PopupSettings
    {
        public PopupType popupType = PopupType.None;
        public Popup popup = null;
    }
    public enum PopupType
    {
        None = 0,
        ResultPopup = 1,
        PausePopup = 2,
        MissionsPopup = 3,
        RetryPopup = 4,
        GameplayPopup = 5,
        SettingsPopup = 6,
        EditNamePopup = 7,
    }

    [SerializeField] private List<PopupSettings> popupSettings = new List<PopupSettings>();
    
    private Dictionary<PopupType, Popup> popupDictionary =  null;

    public override void Init()
    {
        base.Init();

        popupDictionary = new Dictionary<PopupType, Popup>();

        foreach (PopupSettings settings in popupSettings)
        {
            if (!popupDictionary.ContainsKey(settings.popupType))
            {
                popupDictionary.Add(settings.popupType, settings.popup);
            }
        }
    }
    public Popup OpenPopup(PopupType popupType)
    {
        if (!popupDictionary.ContainsKey(popupType))
        {
            Debug.LogErrorFormat("{0} is not found in Popup Dictionary, please check Popup Settings", popupType);
            return null;
        }

        Popup popup = popupDictionary[popupType];
        if (popup != null)
        {
            popup.ReadyPopup();
            popup.OpenPopup();
        }

        return popup;
    }
}
