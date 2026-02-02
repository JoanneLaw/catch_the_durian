using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public string playerId;
    public int highScore;
    public int gem;
    public int totalMissionCompleted = 0;
    public MissionSaveData[] missions = new MissionSaveData[3];
    public PlayerSaveData() {}
    public PlayerSaveData(string id, int hScore, int gemAmount, int missionCompleted = 0, MissionManager.Mission[] allMissions = null)
    {
        playerId = id;
        highScore = hScore;
        gem = gemAmount;
        totalMissionCompleted = missionCompleted;

        if (allMissions != null && allMissions.Length > 0)
        {
            MissionSaveData[] missionSaveDatas = new MissionSaveData[3];
            for (int i = 0; i < allMissions.Length; i++)
            {
                MissionManager.Mission mission = allMissions[i];
                MissionSaveData data = new MissionSaveData();
                data.id = mission.Data.id;
                data.progress = mission.Progress;
                data.isCompleted = mission.IsCompleted;
                data.isClaimed = mission.IsClaimed;
                data.nextMissionTime = mission.NextMissionTime;
                missionSaveDatas[i] = data;
            }

            missions = missionSaveDatas;
        }
    }
}

[Serializable]
public class MissionSaveData
{
    public int id = -1;
    public int progress = 0;
    public bool isCompleted = false;
    public bool isClaimed = false;
    public DateTime nextMissionTime = DateTime.MinValue;
}

[Serializable]
public class BaseServerData
{
    public string playerId;
    public int gameVersion;
}

[Serializable]
public class ClaimMissionData : BaseServerData
{
    public int missionId;
    public int progress;
    public int missionIndex;
}

[Serializable]
public class ServerReturnedPlayerData
{
    public int status;
    public PlayerSaveData data;
}
