using UnityEngine;
using TMPro;
using System;

public class EditNamePopup : Popup
{
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private TextMeshProUGUI placeholderText = null;
    [SerializeField] private TextMeshProUGUI errorText = null;
    [SerializeField] private CustomButton saveButton = null;

    public event Action onNameUpdated;

    public override void ReadyPopup()
    {
        base.ReadyPopup();

        nameInputField.SetTextWithoutNotify(PlayerManager.instance.PlayerName);
        placeholderText.SetText(PlayerManager.instance.PlayerName);
        errorText.gameObject.SetActiveWithCheck(false);
        saveButton.onClick += onSaveButtonClicked;
    }

    private void onSaveButtonClicked()
    {
        string newName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(newName))
        {
            errorText.gameObject.SetActiveWithCheck(true);
            errorText.SetText("Name cannot be empty!");
            return;
        }
        else
        {
            errorText.gameObject.SetActiveWithCheck(false);
        }

        // Update player name
        PlayerManager.instance.UpdatePlayerName(newName, onUpdateNameSuccess, onUpdateNameFailed);
    }

    private void onUpdateNameSuccess()
    {
        onNameUpdated?.Invoke();
        ClosePopup();
        onNameUpdated = null;
    }

    private void onUpdateNameFailed()
    {
        errorText.gameObject.SetActiveWithCheck(true);
        errorText.SetText("Failed to update name. Please try again.");
    }
}
