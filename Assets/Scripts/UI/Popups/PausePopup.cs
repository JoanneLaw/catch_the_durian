using UnityEngine;

public class PausePopup : Popup
{
    [SerializeField] private MissionsPanel missionsPanel = null;
    [SerializeField] private CustomButton quitButton = null;
    [SerializeField] private CustomButton resumeButton = null;

    public override void ReadyPopup()
    {
        base.ReadyPopup();

        missionsPanel.AssignMissions();
    }

    protected override void onPopupOpened()
    {
        base.onPopupOpened();

        quitButton.onClick += onQuitButtonClicked;
        resumeButton.onClick += onResumeButtonClicked;
    }

    public override void ClosePopup()
    {
        base.ClosePopup();

        quitButton.onClick -= onQuitButtonClicked;
        resumeButton.onClick -= onResumeButtonClicked;
    }

    private void onResumeButtonClicked()
    {
        GameManager.instance.Resume();

        ClosePopup();
    }

    private void onQuitButtonClicked()
    {
        GameManager.instance.BackToMainMenu();

        ClosePopup();
    }
}
