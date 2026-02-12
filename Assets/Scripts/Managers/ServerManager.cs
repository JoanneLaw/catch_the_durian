using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

public class ServerManager : MonoSingleton<ServerManager>
{
    public enum PlayerActionStatus
    {
        SUCCESS = 0,
        FAILED = 1,
        SERVER_ERROR = 2,    
    }

    [SerializeField] private GameVersions gameVersions = null;
    public PlayerSaveData PlayerData { get; private set; } = null;
    public bool DataIsLoaded { get; private set; } = false;
    private string apiUrl = "";
    private int versionCode = 0;
    public override void Init()
    {
        base.Init();

        ServerConnection serverConnection = Resources.Load<ServerConnection>("ServerSettings");
        if (serverConnection != null)
        {
            if (serverConnection.useLocalhost)
            {
                apiUrl = "http://localhost:10000";
                Debug.Log("Connected to Localhost!");
            }
            else
            {
                apiUrl = serverConnection.isStaging ? serverConnection.stagingApiUrl : serverConnection.productionApiUrl;   
            }
        }
        
        #if UNITY_ANDROID
            versionCode = gameVersions.androidVersionCode;
        #elif UNITY_IOS
            versionCode = gameVersions.iOSVersionCode;
        #endif
    }

    #region Health Check
    public IEnumerator HealthCheck(Action successCallback, Action failedCallback, Action<int> retryCallback)
    {
        int retryCount = 0;
        int maxRetries = 5;

        while (retryCount < maxRetries)
        {
            UnityWebRequest req = UnityWebRequest.Get(apiUrl + "/health-check");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // Server ready
                successCallback?.Invoke();
                yield break;
            }

            retryCount++;
            retryCallback?.Invoke(retryCount);
            yield return new WaitForSeconds(4f);
        }

        failedCallback?.Invoke();
    }
    #endregion

    #region Missions
    public async Task<bool> ClaimMissionAsync(int missionId, int progress, int missionIndex, Action<ServerReturnedPlayerData> successCallback)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.Log("Api Url is not initialized.");
            return false;
        }

        string json = JsonConvert.SerializeObject(new ClaimMissionData
        {
            playerId = PlayerManager.instance.PlayerId,
            gameVersion = versionCode,
            missionId = missionId,
            progress = progress,
            missionIndex = missionIndex,
        });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/claimMission", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();     // non-blocking wait

            if (request.result == UnityWebRequest.Result.Success)
            {
                ServerReturnedPlayerData returnedData = JsonConvert.DeserializeObject<ServerReturnedPlayerData>(request.downloadHandler.text);
                successCallback?.Invoke(returnedData);
                return true;
            }
            else
            {
                Debug.LogError("Claim mission failed: " + request.error);
                return false;                
            }
        }
    }
    #endregion

    #region Player Game Data
    public async Task UpdatePlayerName(string newName, Action<ServerReturnedPlayerData> onCallback)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.Log("Api Url is not initialized.");
            return;
        }

        string json = JsonConvert.SerializeObject(new PlayerSaveData()
        {
            playerId = PlayerManager.instance.PlayerId,
            playerName = newName,
        });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/updatePlayerName", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();     // non-blocking wait

            if (request.result == UnityWebRequest.Result.Success)
            {
                ServerReturnedPlayerData returnedData = JsonConvert.DeserializeObject<ServerReturnedPlayerData>(request.downloadHandler.text);
                onCallback?.Invoke(returnedData);
            }
            else
            {
                Debug.LogError("Update player name failed: " + request.error);
                onCallback?.Invoke(new ServerReturnedPlayerData()
                {
                    status = (int)PlayerActionStatus.FAILED,
                    data = null
                });
            }
        }
    }

    public void SaveGameData(bool syncServer = true)
    {
        PlayerSaveData data = new PlayerSaveData(
            PlayerManager.instance.PlayerId, 
            PlayerManager.instance.PlayerName,
            PlayerManager.instance.HighScore, 
            PlayerManager.instance.Gem,
            MissionManager.instance.TotalMissionCompleted,
            MissionManager.instance.Missions
        );
        PlayerPrefs.SetString("playerId", data.playerId);
        PlayerPrefs.SetInt("highScore", data.highScore);
        PlayerPrefs.SetInt("gem", data.gem);
        PlayerPrefs.SetInt("totalMissionCompleted", data.totalMissionCompleted);
        
        PlayerPrefs.Save();

        if (syncServer)
            SaveGameDataAsync(data);
    }
    public async void LoadGameDataAsync(Action failedCallback)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.Log("Api Url is not initialized.");
            return;
        }

        string playerId = PlayerPrefs.GetString("playerId");
        ServerReturnedPlayerData returnedData = await LoadData(playerId);

        if (returnedData == null)
        {
            failedCallback?.Invoke();
        }
        else
        {
            PlayerActionStatus status = (PlayerActionStatus)returnedData.status;

            if (status == PlayerActionStatus.FAILED)
            {
                failedCallback?.Invoke();
            }
            else
            {
                PlayerData = returnedData.data;
                DataIsLoaded = true;   
            }   
        }
    }
    private async void SaveGameDataAsync(PlayerSaveData playerSaveData)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.Log("Api Url is not initialized.");
            return;
        }

        await SaveData(playerSaveData);
    }
    // Save Player Data
    private async Task SaveData(PlayerSaveData playerSaveData)
    {
        string json = JsonConvert.SerializeObject(playerSaveData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/savePlayerData", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();     // non-blocking wait

            if (request.result == UnityWebRequest.Result.Success)
                Debug.Log("Data saved successfully.");
            else
                Debug.LogError("Save failed: " + request.error);
        }
    }
    // Load Player Save Data
    private async Task<ServerReturnedPlayerData> LoadData(string playerId)
    {
        PlayerSaveData playerSaveData = new PlayerSaveData()
        {
            playerId = playerId
        };
        string json = JsonConvert.SerializeObject(playerSaveData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "/loadPlayerData", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (request.result == UnityWebRequest.Result.Success)
            {
                string resultJson = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<ServerReturnedPlayerData>(resultJson);
            } else
            {
                Debug.LogError("Load failed: " + request.error);
                return null;
            }
        }
    }
    #endregion
}
