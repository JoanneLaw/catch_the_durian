using UnityEngine;
using TMPro;

public class SettingsPopup : Popup
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private CustomButton editButton;
    [SerializeField] private CustomButton copyButton;

    public override void ReadyPopup()
    {
        base.ReadyPopup();

        playerNameText.SetText(PlayerManager.instance.PlayerName);
        playerIdText.SetText(PlayerManager.instance.PlayerId);

        editButton.onClick += onEditButtonClicked;
        copyButton.onClick += onCopyButtonClicked;
    }

    private void onEditButtonClicked()
    {
        EditNamePopup editNamePopup = PopupManager.instance.OpenPopup(PopupManager.PopupType.EditNamePopup) as EditNamePopup;
        editNamePopup.onNameUpdated += OnNameUpdated;
    }

    private void OnNameUpdated()
    {
        playerNameText.SetText(PlayerManager.instance.PlayerName);
    }

    private void onCopyButtonClicked()
    {
        GUIUtility.systemCopyBuffer = PlayerManager.instance.PlayerId;

        GameManager.instance.UiManager.PlayFloatingMessage("Player ID copied to clipboard!");   
    }
}
