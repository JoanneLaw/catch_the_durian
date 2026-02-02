using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mission Data")]
public class MissionData : ScriptableObject
{
    public int id;
    public MissionManager.MissionType missionType = MissionManager.MissionType.None;
    public string description;
    public int target;
}
