using TMPro;
using UnityEngine;

public class MissionsPopup : Popup
{
    [SerializeField] private MissionsPanel missionsPanel = null;

    public override void ReadyPopup()
    {
        base.ReadyPopup();

        missionsPanel.AssignMissions();
    }
}
