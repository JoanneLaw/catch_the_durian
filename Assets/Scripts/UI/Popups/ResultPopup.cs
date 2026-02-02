using TMPro;
using UnityEngine;

public class ResultPopup : Popup
{
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private CustomButton restartButton = null;
    [SerializeField] private CustomButton quitButton = null;
    [SerializeField] private CustomButton missionButton = null;

    public override void ReadyPopup()
    {
        base.ReadyPopup();

        scoreText.SetText(PlayerManager.instance.Score.ToString());
    }

    public override void ClosePopup()
    {
        base.ClosePopup();

        restartButton.onClick -= onRestartClicked;
        quitButton.onClick -= onQuitClicked;
        missionButton.onClick -= onMissionButtonClicked;
    }

    protected override void onPopupOpened()
    {
        base.onPopupOpened();

        restartButton.onClick += onRestartClicked;
        quitButton.onClick += onQuitClicked;
        missionButton.onClick += onMissionButtonClicked;
    }

    private void onRestartClicked()
    {
        GameManager.instance.Restart();

        ClosePopup();
    }

    private void onQuitClicked()
    {
        GameManager.instance.BackToMainMenu();

        ClosePopup();
    }

    private void onMissionButtonClicked()
    {
        PopupManager.instance.OpenPopup(PopupManager.PopupType.MissionsPopup);
    }
}
