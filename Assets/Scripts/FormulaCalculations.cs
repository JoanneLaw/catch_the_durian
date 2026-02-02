using UnityEngine;

public static class FormulaCalculations
{
    public static int GetScore(Item.Type durianType)
    {
        switch (durianType)
        {
            case Item.Type.Normal:
                return GlobalDef.normalScore * GetMissionScoreMultiplier();
            case Item.Type.Premium:
                return GlobalDef.premiumScore * GetMissionScoreMultiplier();
        }

        return 0;
    }

    public static int GetMissionScoreMultiplier(bool clampValue = true)
    {
        if (clampValue)
            return Mathf.Clamp(Mathf.FloorToInt(MissionManager.instance.TotalMissionCompleted / GlobalDef.missionTarget) + 1, 1, GlobalDef.maxMissionScoreMultiplier);

        return Mathf.FloorToInt(MissionManager.instance.TotalMissionCompleted / GlobalDef.missionTarget) + 1;
    }
}
