using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MissionsPanel : MonoBehaviour
{
    [SerializeField] private ProgressBar totalMissionProgressBar = null;
    [SerializeField] private Image progressBarFlash = null;
    [SerializeField] private Image scoreMultiplierFlash = null;
    [SerializeField] private TextMeshProUGUI scoreMultiplierText = null;
    [SerializeField] private GameObject uiBlocker = null;
    [SerializeField] private MissionPanel[] missionPanels = null;

    private Sequence totalMissionSeq;

    private void Awake()
    {
        for (int i = 0; i < missionPanels.Length; i++)
        {
            missionPanels[i].onMissionClaimed += onMissionClaimed;
        }
    }

    public void AssignMissions()
    {
        progressBarFlash.SetAlpha(0);
        scoreMultiplierFlash.SetAlpha(0);
        scoreMultiplierText.SetText($"x{FormulaCalculations.GetMissionScoreMultiplier()}");
        if (MissionManager.instance.IsMaxMultiplier)
            totalMissionProgressBar.SetText("MAX");
        else
            totalMissionProgressBar.UpdateProgress(MissionManager.instance.TotalMissionCompleted % GlobalDef.missionTarget, GlobalDef.missionTarget);

        for (int i = 0; i < missionPanels.Length; i++)
        {
            MissionManager.Mission mission = MissionManager.instance.Missions[i];
            missionPanels[i].AssignMission(mission, i);
        }
    }

    private void onMissionClaimed()
    {
        if (MissionManager.instance.TotalMissionCompleted % GlobalDef.missionTarget == 0)
        {
            if (FormulaCalculations.GetMissionScoreMultiplier(false) <= GlobalDef.maxMissionScoreMultiplier)
            {
                uiBlocker.SetActiveWithCheck(true);
                totalMissionSeq = Sequence.Create(useUnscaledTime: true)
                    .Group(totalMissionProgressBar.UpdateProgressAnimate(GlobalDef.missionTarget, GlobalDef.missionTarget, 0.2f, false, true))
                    .Chain(Tween.Alpha(progressBarFlash, 1f, 0.2f))
                    .Group(Tween.Alpha(scoreMultiplierFlash, 1f, 0.2f))
                    .ChainDelay(0.2f)
                    .ChainCallback(() => 
                    {
                        scoreMultiplierText.SetText($"x{FormulaCalculations.GetMissionScoreMultiplier()}");
                        if (MissionManager.instance.IsMaxMultiplier)
                            totalMissionProgressBar.SetText("MAX");
                        else
                            totalMissionProgressBar.UpdateProgress(MissionManager.instance.TotalMissionCompleted % GlobalDef.missionTarget, GlobalDef.missionTarget);
                    })
                    .Chain(Tween.Alpha(progressBarFlash, 0f, 0.2f))
                    .Group(Tween.Alpha(scoreMultiplierFlash, 0f, 0.2f))
                    .OnComplete(() => uiBlocker.SetActiveWithCheck(false));
            }
        }
        else
        {
            if (!MissionManager.instance.IsMaxMultiplier)
            {
                totalMissionSeq = Sequence.Create(useUnscaledTime: true)
                    .Group(totalMissionProgressBar.UpdateProgressAnimate(MissionManager.instance.TotalMissionCompleted % GlobalDef.missionTarget, GlobalDef.missionTarget, 0.2f, false, true));
            }
        }   
    }
}
