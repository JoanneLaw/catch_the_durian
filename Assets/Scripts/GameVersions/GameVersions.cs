using UnityEngine;

[CreateAssetMenu(fileName = "GameVersions", menuName = "Game Versions")]
public class GameVersions : ScriptableObject
{
    public int androidVersionCode;
    public int iOSVersionCode;
}
