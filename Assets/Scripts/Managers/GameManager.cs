using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        MainMenu = 0,
        Play,           // In Play mode, players haven't drag to start
        Pause,
        GameOver,
        Start,          // Game is started
        FeverMode,

    }
    [SerializeField] private UIManager uiManager = null;
    public UIManager UiManager => uiManager;
    public GameState PrevState { get; private set; } = GameState.MainMenu;
    private GameState _state = GameState.MainMenu;
    public GameState State 
    {
        get => _state;
        private set
        {
            PrevState = _state;
            _state = value;
        }
    }
    public override void Init()
    {
        base.Init();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        uiManager.Init();
    }
#if UNITY_EDITOR
    private InputAction saveDebug;
    private InputAction pauseDebug;
    private void OnEnable()
    {
        saveDebug = new InputAction("SaveGame", binding: "<Keyboard>/s");
        pauseDebug = new InputAction("PauseGame", binding: "<Keyboard>/p");

        saveDebug.started += onSaveDebug;
        pauseDebug.started += onPauseDebug;

        saveDebug.Enable();
        pauseDebug.Enable();
    }
    private void OnDisable()
    {
        saveDebug.started -= onSaveDebug;
        pauseDebug.started -= onPauseDebug;

        saveDebug.Disable();
        pauseDebug.Disable();
    }
    private void onSaveDebug(InputAction.CallbackContext callback)
    {
        ServerManager.instance.SaveGameData();
    }
    private void onPauseDebug(InputAction.CallbackContext callback)
    {
        if (State == GameState.Pause)
            Resume();
        else
        {
            Pause();
            uiManager.ToggleInGameTweens(true);
        }
    }
#endif

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            ServerManager.instance.SaveGameData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            Application.targetFrameRate = 60;
    }

    public void GameOver()
    {
        State = GameState.GameOver;

        uiManager.StopInGameTweens();
        PopupManager.instance.OpenPopup(PopupManager.PopupType.ResultPopup);
    }

    public void Restart()
    {
        State = GameState.Play;
        PlayerManager.instance.Restart();
        ItemManager.instance.Restart();
        uiManager.Restart();

        MissionManager.instance.TriggerMission(MissionManager.MissionType.GameCount, 1);
    }

    public void StartGame()
    {
        State = GameState.Start;
        uiManager.UpdateStartText(false);
    }

    public void Pause()
    {
        State = GameState.Pause;
    }

    public void Resume()
    {
        if (PrevState == GameState.Play)
        {
            State = GameState.Play;
            return;
        }

        StartCoroutine(ReadyResume());
    }

    private IEnumerator ReadyResume()
    {
        uiManager.SetPauseButtonState(false);
        for (int i = 3; i > 0; i--)
        {
            uiManager.PlayCountdown(i);
            yield return new WaitForSeconds(1f);
        }

        uiManager.HideCountdown();
        uiManager.SetPauseButtonState(true);
        uiManager.ToggleInGameTweens(false);
        State = PrevState;
    }

    public void StartFeverMode()
    {
        State = GameState.FeverMode;
    }

    public void EndFeverMode()
    {
        State = GameState.Start;
    }

    public void BackToMainMenu()
    {
        State = GameState.MainMenu;
        uiManager.ChangeScene(true);
    }
}
