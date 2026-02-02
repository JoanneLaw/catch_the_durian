using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionManager : MonoSingleton<MissionManager>
{
    public enum MissionType
    {
        None = 0,
        GameCount = 1,
        Score = 2,
        FeverModeCount = 3,
        GoodDurianCount = 4,
        PremiumDurianCount = 5,
    }

    [SerializeField] private MissionData[] missionDatas = null;
    
    public int TotalMissionCompleted { get; private set; } = 0;
    public Mission[] Missions { get; private set; } = new Mission[3];
    public bool IsMaxMultiplier => FormulaCalculations.GetMissionScoreMultiplier() >= GlobalDef.maxMissionScoreMultiplier;
    private bool allMissionsCompleted = false;
    public event Action<PlayerSaveData> onMissionClaimed;
    public override void Init()
    {
        base.Init();

        LoadData(ServerManager.instance.PlayerData);

        // #if GAME_DEBUG_MODE

        // for (int i = 0; i < Missions.Length; i++)
        // {
        //     Mission mission = Missions[i];
        //     Debug.Log($"(Init) [{mission.GetMissionType()}] progress: {mission.Progress}, target: {mission.Data.target}");
        // }

        // #endif
    }

    public void TriggerMission(MissionType missionType, int progress)
    {
        if (allMissionsCompleted)
            return;

        int missionCompletedCount = 0;
        for (int i = 0; i < Missions.Length; i++)
        {
            Mission mission = Missions[i];
            if (mission.GetMissionType() == missionType)
            {
                mission.UpdateProgress(progress);
                // Debug.Log($"[{mission.GetMissionType()}] progress: {mission.Progress}, target: {mission.Data.target}");
            }

            if (mission.IsCompleted)
                missionCompletedCount++;
        }

        if (missionCompletedCount >= GlobalDef.maxMission)
            allMissionsCompleted = true;
    }

    public Mission GenerateNewMission(int index)
    {
        MissionData newMission = missionDatas[UnityEngine.Random.Range(0, missionDatas.Length)];
        Missions[index] = new Mission(newMission);
        return Missions[index];
    }

    private MissionData GetMissionDataById(int id)
    {
        for (int i = 0; i < missionDatas.Length; i++)
        {
            if (id == missionDatas[i].id)
                return missionDatas[i];
        }

        return null;
    }

    private void CheckMissionCompletion()
    {
        int completedCount = 0;
        for (int i = 0; i < Missions.Length; i++)
        {
            if (Missions[i].IsCompleted)
                completedCount++;
        }

        allMissionsCompleted = completedCount >= GlobalDef.maxMission;            
    }

    public async void ClaimMission(Mission mission, int missionIndex, Action<PlayerSaveData> successCallback = null)
    {
        GameManager.instance.UiManager.ShowLoadingOverlay();

        bool requestSuccess = await ServerManager.instance.ClaimMissionAsync(mission.Data.id, mission.Progress, missionIndex, (returnedData) => 
        {
            if ((ServerManager.PlayerActionStatus)returnedData.status == ServerManager.PlayerActionStatus.SUCCESS)
            {
                PlayerSaveData playerSaveData = returnedData.data;
                Missions[missionIndex] = new Mission(playerSaveData.missions[missionIndex]);
                TotalMissionCompleted = playerSaveData.totalMissionCompleted;
                onMissionClaimed?.Invoke(playerSaveData);
                successCallback?.Invoke(playerSaveData);
            }
            else
            {
                RetryPopup popup = PopupManager.instance.OpenPopup(PopupManager.PopupType.RetryPopup) as RetryPopup;
                popup.SetDesc("Failed to claim mission. Please try again later.");
            }
        });

        if (!requestSuccess)
        {
            RetryPopup popup = PopupManager.instance.OpenPopup(PopupManager.PopupType.RetryPopup) as RetryPopup;
            popup.SetDesc("Failed to claim mission. Please try again later.");
        }

        GameManager.instance.UiManager.HideLoadingOverlay();
    }

    private void LoadData(PlayerSaveData data)
    {
        TotalMissionCompleted = data.totalMissionCompleted;

        Missions = new Mission[GlobalDef.maxMission];
        MissionSaveData[] missionSaveDatas = data.missions;

        for (int i = 0; i < missionSaveDatas.Length; i++)
        {
            MissionSaveData missionSaveData = missionSaveDatas[i];
            MissionData missionData = GetMissionDataById(missionSaveData.id);
            if (missionData != null)
            {
                Mission mission = new Mission(missionData, missionSaveData.progress, missionSaveData.isCompleted, missionSaveData.isClaimed, missionSaveData.nextMissionTime);
                Missions[i] = mission;
            }
            else
            {
                GenerateNewMission(i);
            }
        }

        CheckMissionCompletion();
    }

    public class Mission
    {
        public MissionData Data { get; private set; } = null;
        public int Progress { get; private set; } = 0;
        public bool IsCompleted { get; private set; } = false;
        public bool IsClaimed { get; private set; } = false;
        public DateTime NextMissionTime { get; private set; } = DateTime.MinValue;
        public Mission(MissionSaveData saveData)
        {
            Data = instance.GetMissionDataById(saveData.id);
            Progress = saveData.progress;
            IsCompleted = saveData.isCompleted;
            IsClaimed = saveData.isClaimed;
            NextMissionTime = saveData.nextMissionTime;
        }
        public Mission(MissionData missionData)
        {
            Data = missionData;
            Progress = 0;
            IsCompleted = false;
            IsClaimed = false;
            NextMissionTime = DateTime.UtcNow;
        }
        public Mission(MissionData missionData, int progress, bool isCompleted, bool isClaimed, DateTime nextMissionTime)
        {
            Data = missionData;
            Progress = progress;
            IsCompleted = isCompleted;
            IsClaimed = isClaimed;
            NextMissionTime = nextMissionTime;
        }

        public MissionType GetMissionType()
        {
            if (Data != null)
                return Data.missionType;
            
            return MissionType.None;
        }

        public string GetDescText()
        {
            if (Data.description == null)
                return "";
                
            return string.Format(Data.description, $"<color=yellow>{Data.target}</color>");
        }

        public void UpdateProgress(int amount)
        {
            if (Progress < Data.target)
            {
                Progress += amount;

                if (Progress >= Data.target && !IsCompleted)
                {
                    IsCompleted = true;
                }
            }
        }
    }
}
