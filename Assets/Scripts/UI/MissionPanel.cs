using System;
using TMPro;
using UnityEngine;

public class MissionPanel : MonoBehaviour
{
    [SerializeField] private GameObject missionContent = null;
    [SerializeField] private GameObject countdownContent = null;
    [SerializeField] private TextMeshProUGUI countdownText = null;
    [SerializeField] private TextMeshProUGUI titleText = null;
    [SerializeField] private ProgressBar progressBar = null;
    [SerializeField] private TextMeshProUGUI rewardAmountText = null;
    [SerializeField] private CustomButton claimButton = null;

    public event Action onMissionClaimed;
    private MissionManager.Mission missionData = null;
    private int missionIndex = -1;
    private bool isCountingdown = false;
    private void Awake()
    {
        claimButton.onClick += onClaimButtonClicked;
    }

    private void Update()
    {
        if (isCountingdown)
        {
            TimeSpan timeSpan = missionData.NextMissionTime.Subtract(DateTime.UtcNow);
            countdownText.SetText(DateTimeEx.ConvertDuration(timeSpan));

            if (timeSpan.TotalSeconds <= 0)
            {
                isCountingdown = false;
                UpdateMission();
            }
        }
    }

    private void onClaimButtonClicked()
    {
        MissionManager.instance.ClaimMission(missionData, missionIndex, (returnedData) => 
        {
            AssignMission(new MissionManager.Mission(returnedData.missions[missionIndex]), missionIndex);

            onMissionClaimed?.Invoke();
        });
    }

    public void AssignMission(MissionManager.Mission mission, int index)
    {
        missionIndex = index;
        missionData = mission;

        UpdateMission();
    }

    private void UpdateMission()
    {
        if (missionData.IsCompleted && missionData.IsClaimed)
        {
            if (missionData.NextMissionTime <= DateTime.UtcNow)
            {
                // generate new mission
                missionData = MissionManager.instance.GenerateNewMission(missionIndex);
                isCountingdown = false;
            }
            else
            {
                // show countdown
                isCountingdown = true;
            }
        }
        else
        {
            isCountingdown = false;
        }

        if (isCountingdown)
        {
            missionContent.SetActiveWithCheck(false);
            countdownContent.SetActiveWithCheck(true);
        }
        else
        {
            UpdateMissionContent();
            missionContent.SetActiveWithCheck(true);
            countdownContent.SetActiveWithCheck(false);
        }
    }
    
    private void UpdateMissionContent()
    {
        titleText.SetText(missionData.GetDescText());
        progressBar.UpdateProgress(missionData.Progress, missionData.Data.target);
        rewardAmountText.SetText($"x{GlobalDef.missionGemReward}");

        progressBar.IsEnabled = false;
        claimButton.IsEnabled = false;

        if (missionData.Progress >= missionData.Data.target)
        {
            progressBar.IsEnabled = true;
            claimButton.IsEnabled = true;
        }
    }
}
