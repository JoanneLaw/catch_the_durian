using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Camera uiCamera = null;
    [SerializeField] private RectTransform canvasRect = null;
    [SerializeField] private GameObject inGameUI = null;
    [SerializeField] private GameObject inGameScene = null;
    [SerializeField] private GameObject mainMenuUI = null;
    [SerializeField] private GameObject loadingOverlay = null;

    [Title("Main Menu", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private CustomButton playButton = null;
    [SerializeField] private CustomButton missionButton = null;
    [SerializeField] private CustomButton highScoreButton = null;
    [SerializeField] private CustomButton settingsButton = null;
    [SerializeField] private CustomButton shopButton = null;
    [SerializeField] private CustomButton gameplayButton = null;

    [Title("In Game", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private ImageToggle[] hpImgs = null;
    [SerializeField] private Image[] dropHpEffect = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI highScoreText = null;
    [SerializeField] private SpriteData spriteData = null;
    [SerializeField] private ProgressBar feverModeProgressBar = null;
    [SerializeField] private CustomButton pauseButton = null;
    [SerializeField] private TextMeshProUGUI countdownText = null;

    [Title("", "VFX")]
    [SerializeField] private FadeEffect startTextEffect = null;
    [SerializeField] private TextMeshProUGUI floatingMsg = null;

    [Title("", "Debug")]
    [SerializeField] private GameObject debugUI = null;
    [SerializeField] private TextMeshProUGUI debugScoreText = null;

    public SpriteData SprData => spriteData;
    private Sequence collectedAmountSeq;
    private Vector2 minScreenWorldPos;
    private Vector2 maxScreenWorldPos;
    private Sequence floatingMsgSeq;
    public Vector2 MinScreenWorldPos => minScreenWorldPos;
    public Vector2 MaxScreenWorldPos => maxScreenWorldPos;
    public Camera UiCamera => uiCamera;
    public RectTransform CanvasRect => canvasRect;
    public void Init()
    {
        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        maxScreenWorldPos = new Vector2(width, height);
        minScreenWorldPos = new Vector2(-width, -height);

        ChangeScene(true);
        
        PlayerManager.instance.onChanceUpdated += UpdateChancesUI;
        PlayerManager.instance.onScoreUpdated += UpdateScoreUI;
        PlayerManager.instance.onCollectedAmountUpdated += onCollectedAmountUpdated;

        playButton.onClick += onPlayButtonClicked;
        pauseButton.onClick += onPauseButtonClicked;
        missionButton.onClick += onMissionButtonClicked;
        highScoreButton.onClick += onHighScoreButtonClicked;
        settingsButton.onClick += onSettingsButtonClicked;
        shopButton.onClick += onShopButtonClicked;
        gameplayButton.onClick += onGameplayButtonClicked;

        // Debug
#if GAME_DEBUG_MODE
        debugUI.SetActiveWithCheck(true);
        ResetDebugScore();
#else
        debugUI.SetActiveWithCheck(false);
#endif
    }

    private void OnDestroy()
    {
        if (PlayerManager.IsInstanceReady)
        {
            PlayerManager.instance.onChanceUpdated -= UpdateChancesUI;
            PlayerManager.instance.onScoreUpdated -= UpdateScoreUI;
            PlayerManager.instance.onCollectedAmountUpdated -= onCollectedAmountUpdated;
        }
    }

    #region Buttons
    private void onPlayButtonClicked()
    {
        ChangeScene(false);
        GameManager.instance.Restart();
    }

    private void onPauseButtonClicked()
    {
        GameManager.instance.Pause();
        ToggleInGameTweens(true);
        ServerManager.instance.SaveGameData();
        PopupManager.instance.OpenPopup(PopupManager.PopupType.PausePopup);
    }

    private void onMissionButtonClicked()
    {
        PopupManager.instance.OpenPopup(PopupManager.PopupType.MissionsPopup);
    }

    private void onHighScoreButtonClicked()
    {
        PlayFloatingMessage("Leaderboard not yet implemented.");
    }

    private void onSettingsButtonClicked()
    {
        PopupManager.instance.OpenPopup(PopupManager.PopupType.SettingsPopup);
    }

    private void onShopButtonClicked()
    {
        PlayFloatingMessage("Shop not yet implemented.");
    }

    private void onGameplayButtonClicked()
    {
        PopupManager.instance.OpenPopup(PopupManager.PopupType.GameplayPopup);
    }
    #endregion

    public void PlayFloatingMessage(string msg)
    {
        floatingMsgSeq.Stop();

        floatingMsg.rectTransform.anchoredPosition = new Vector2(0f, 200f);
        floatingMsg.SetText(msg);
        floatingMsg.SetAlpha(0f);

        floatingMsg.gameObject.SetActiveWithCheck(true);

        floatingMsgSeq = Sequence.Create(useUnscaledTime: true)
            .Group(Tween.Alpha(floatingMsg, 1f, 0.2f))
            .Group(Tween.UIAnchoredPositionY(floatingMsg.rectTransform, floatingMsg.rectTransform.anchoredPosition.y + 150f, 1f))
            .Chain(Tween.Alpha(floatingMsg, 0f, 0.3f))
            .OnComplete(() => floatingMsg.gameObject.SetActiveWithCheck(false));
    }

    private void UpdateChancesUI(bool animateDropHp)
    {
        for (int i = 0; i < PlayerManager.instance.Chance; i++)
        {
            if (hpImgs.Length > i)
            {
                hpImgs[i].IsEnabled = true;
            }
        }

        for (int i = PlayerManager.instance.Chance; i < hpImgs.Length; i++)
        {
            hpImgs[i].IsEnabled = false;

            // Animate hp drop effect
            if (animateDropHp)
            {
                if (i == PlayerManager.instance.Chance)
                {
                    int index = i;
                    dropHpEffect[index].transform.localPosition = Vector3.zero;
                    dropHpEffect[index].SetAlpha(1f);
                    dropHpEffect[index].gameObject.SetActiveWithCheck(true);
                    Sequence.Create(sequenceEase: Ease.OutQuint, useUnscaledTime: true)
                        .Group(Tween.LocalPositionY(dropHpEffect[index].transform, -130f, 2f))
                        .Group(Tween.Alpha(dropHpEffect[index], 0f, 2f))
                        .OnComplete(() => dropHpEffect[index].gameObject.SetActiveWithCheck(false));
                }
            }
        }
    }
    public void Restart()
    {
        UpdateChancesUI(false);
        UpdateScoreUI();
        UpdateStartText(true);
        feverModeProgressBar.ResetProgress();
    }
    public void StopInGameTweens()
    {
        if (collectedAmountSeq.isAlive)
            collectedAmountSeq.Stop();
    }
    public void ToggleInGameTweens(bool pause)
    {
        if (collectedAmountSeq.isAlive && collectedAmountSeq.isPaused != pause)
        {
            collectedAmountSeq.isPaused = pause;
        }
    }
    public void UpdateStartText(bool show)
    {
        startTextEffect.gameObject.SetActiveWithCheck(show);
    }
    public void UpdateHighScoreText()
    {
        highScoreText.SetText(PlayerManager.instance.HighScore.ToString("N0"));
    }
    private void UpdateScoreUI()
    {
        scoreText.SetText(PlayerManager.instance.Score.ToString());
    }

    private void onCollectedAmountUpdated()
    {
        collectedAmountSeq = Sequence.Create()
            .Group(feverModeProgressBar.UpdateProgressAnimate(PlayerManager.instance.CollectedAmount, GlobalDef.feverModeThreshold));

        if (PlayerManager.instance.IsFeverTime)
        {
            collectedAmountSeq
                .ChainCallback(() => GameManager.instance.StartFeverMode())
                .Chain(feverModeProgressBar.PlayProgressBarGoesToZero(GlobalDef.feverModeDuration))
                .OnComplete(target: this, target => target.onFeverModeEnded());
        }
    }

    private void onFeverModeEnded()
    {
        PlayerManager.instance.ResetCollectedAmount();
        GameManager.instance.EndFeverMode();
    }

    public void ChangeScene(bool goToMainMenu)
    {
        inGameScene.SetActiveWithCheck(!goToMainMenu);
        inGameUI.SetActiveWithCheck(!goToMainMenu);
        mainMenuUI.SetActiveWithCheck(goToMainMenu);

        // Show Gameplay Popup for new player
        if (!goToMainMenu)
        {
            if (PlayerPrefs.GetInt("isNewPlayer", 1) == 1)
            {
                PopupManager.instance.OpenPopup(PopupManager.PopupType.GameplayPopup);
                PlayerPrefs.SetInt("isNewPlayer", 0);
            }
        }
    }

    public void SetPauseButtonState(bool enabled)
    {
        pauseButton.IsEnabled = enabled;
    }

    public void ShowLoadingOverlay()
    {
        loadingOverlay.SetActiveWithCheck(true);
    }

    public void HideLoadingOverlay()
    {
        loadingOverlay.SetActiveWithCheck(false);
    }

    #region Animation

    public void PlayCountdown(int count)
    {
        countdownText.gameObject.SetActiveWithCheck(true);
        countdownText.rectTransform.localScale = Vector3.zero;
        countdownText.SetText(count.ToString());
        Tween.Scale(countdownText.rectTransform, Vector3.one, 0.2f, Ease.OutBack);
    }

    public void HideCountdown()
    {
        countdownText.gameObject.SetActiveWithCheck(false);
    }

    #endregion

    #region Debug Code

    public void UpdateDebugScore(List<int> scoreList)
    {
#if GAME_DEBUG_MODE
        string debugScoreTxt = "";
        for (int i = 0; i < scoreList.Count; i++)
        {
            debugScoreTxt += $"Stage {i + 1}: {scoreList[i]}\n";
        }
        debugScoreText.SetText(debugScoreTxt);
#endif
    }

    public void ResetDebugScore()
    {
#if GAME_DEBUG_MODE
        debugScoreText.SetText("");
#endif
    }

    #endregion
}
