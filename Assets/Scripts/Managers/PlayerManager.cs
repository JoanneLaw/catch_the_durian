using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] private Player player = null;

    private List<int> scoreList = new List<int>();

    private int chance;
    public int Chance
    {
        get => chance;
        private set
        {
            if (value > GlobalDef.playerChance)
                return;

            int prevChance = chance;
            chance = value;
            if (chance <= 0)
            {
                Die();
            }

            onChanceUpdated?.Invoke(prevChance > chance);
        }
    }

    private int score;
    public int Score
    {
        get => score;
        private set
        {
            score = value;

            onScoreUpdated?.Invoke();
        }
    }

    private string playerName = "";
    public string PlayerName
    {
        get => playerName;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                playerName = "Farmer" + PlayerId.Substring(PlayerId.Length - 6, 6);
                return;
            }
            
            playerName = value;
        }
    }
    public string PlayerId { get; private set; } = "";
    public int HighScore { get; private set; } = 0;
    public int Gem { get; private set; } = 0;
    public int CollectedAmount { get; private set; } = 0;
    public bool IsFeverTime { get; private set; } = false;
    public event Action<bool> onChanceUpdated;
    public event Action onScoreUpdated;
    public event Action onCollectedAmountUpdated;
    public event Action onGemUpdated;
    public override void Init()
    {
        base.Init();

        Chance = GlobalDef.playerChance;
        Score = 0;

        LoadData(ServerManager.instance.PlayerData);
        ServerManager.instance.SaveGameData(false);

        MissionManager.instance.onMissionClaimed += OnMissionClaimed;
    }

    private void OnMissionClaimed(PlayerSaveData returnedData)
    {
        Gem = returnedData.gem;
        
        onGemUpdated?.Invoke();
    }

    public void AddChance (int amount)
    {
        Chance += amount;
    }

    public void AddScore (int amount, int stage, bool excludeFromDebugging = false)
    {
        if (!excludeFromDebugging)
        {
            if (scoreList.Count < stage)
            {
                scoreList.Add(0);
            }

            scoreList[stage - 1] += amount;
            GameManager.instance.UiManager.UpdateDebugScore(scoreList);
        }

        Score += amount;

        MissionManager.instance.TriggerMission(MissionManager.MissionType.Score, amount);
    }

    public void AddCollectedAmount(int amount)
    {
        if (IsFeverTime)
            return;
        
        CollectedAmount += amount;

        if (CollectedAmount >= GlobalDef.feverModeThreshold)
        {
            IsFeverTime = true;

            MissionManager.instance.TriggerMission(MissionManager.MissionType.FeverModeCount, 1);
        }

        onCollectedAmountUpdated?.Invoke();
    }

    public void ResetCollectedAmount()
    {
        CollectedAmount = 0;
        IsFeverTime = false;
    }

    public void AddGem(int amount)
    {
        Gem += amount;

        onGemUpdated?.Invoke();
    }
    private void Die ()
    {
        if (HighScore < Score)
        {
            HighScore = Score;
            GameManager.instance.UiManager.UpdateHighScoreText();
        }
        GameManager.instance.GameOver();
        ServerManager.instance.SaveGameData();
    }

    public void Restart()
    {
        player.Restart();
        Chance = GlobalDef.playerChance;
        Score = 0;
        scoreList.Clear();
        ResetCollectedAmount();
    }
    private void LoadData(PlayerSaveData data)
    {
        PlayerId = data.playerId;
        HighScore = data.highScore;
        Gem = data.gem;
        PlayerName = data.playerName;

        GameManager.instance.UiManager.UpdateHighScoreText();
    }

    public async void UpdatePlayerName(string newName, Action onSuccessCallback, Action onFailedCallback)
    {
        GameManager.instance.UiManager.ShowLoadingOverlay();

        await ServerManager.instance.UpdatePlayerName(newName, (returnedData) =>
        {
            switch((ServerManager.PlayerActionStatus)returnedData.status)
            {
                case ServerManager.PlayerActionStatus.SUCCESS:
                    PlayerName = returnedData.data.playerName;
                    onSuccessCallback?.Invoke();
                    break;
                case ServerManager.PlayerActionStatus.FAILED:
                    onFailedCallback?.Invoke();
                    break;
            }
        });

        GameManager.instance.UiManager.HideLoadingOverlay();
    }
}
