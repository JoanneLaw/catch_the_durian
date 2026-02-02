using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private ProgressBar loadingProgress = null;
    [SerializeField] private TextMeshProUGUI playerIdText = null;
    [SerializeField] private TextMeshProUGUI loadingText = null;
    [SerializeField] private RetryPopup retryPopup = null;

    private bool serverIsAlive = false;
    private void Awake()
    {
        playerIdText.SetText("");
        loadingProgress.ResetProgress();

        StartCoroutine(Load());   
    }

    IEnumerator Load()
    {
        loadingText.SetText("");
        loadingProgress.UpdateProgressAnimate(0.2f, 1f, 0.5f, isNested: false);

        yield return new WaitForSeconds(0.5f);

        // Server startup
        StartupServer();

        yield return new WaitUntil(() => serverIsAlive);

        loadingProgress.UpdateProgressAnimate(0.4f, 1f, 0.5f, isNested: false);

        yield return new WaitForSeconds(0.5f);

        // Load Game Data
        loadingText.SetText("Loading game data...");
        LoadGameData();

        yield return new WaitUntil(() => ServerManager.instance.DataIsLoaded);

        playerIdText.SetText(ServerManager.instance.PlayerData.playerId);
        loadingProgress.UpdateProgressAnimate(0.6f, 1f, 0.5f, isNested: false);

        yield return new WaitForSeconds(0.5f);

        // Load Main scene
        loadingText.SetText("Entering game...");
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        loadSceneAsync.allowSceneActivation = false;

        while (loadSceneAsync.progress < 0.9f)
            yield return null;

        loadingProgress.UpdateProgressAnimate(1f, 1f, 0.5f, isNested: false);

        yield return new WaitForSeconds(0.5f);

        // Activate Main Scene and unload Loading Scene
        loadSceneAsync.allowSceneActivation = true;

        while (!loadSceneAsync.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainScene"));
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    private void StartupServer()
    {
        loadingText.SetText("Connecting to game services...");
        StartCoroutine(ServerManager.instance.HealthCheck(() =>
        {
            serverIsAlive = true;
        }, () =>
        {
            retryPopup.SetDesc("Server is taking longer than expected to start due to free hosting. Please try again.");
            retryPopup.ReadyPopup();
            retryPopup.OpenPopup();
            retryPopup.onRetry += StartupServer;
        }, (retryCount) =>
        {
            loadingText.SetText($"Connecting to game services... Retry {retryCount}");
        }));
    }

    private void LoadGameData()
    {
        ServerManager.instance.LoadGameDataAsync(() =>
        {
            retryPopup.SetDesc("Fail to load player data. Please try again.");
            retryPopup.ReadyPopup();
            retryPopup.OpenPopup();
            retryPopup.onPopupClosedCallback += LoadGameData;
        });
    }
}
