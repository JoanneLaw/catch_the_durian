using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerConnectionData", menuName = "Server Conenction Data")]
public class ServerConnection : ScriptableObject
{
    public string productionApiUrl;
    public string stagingApiUrl;
    public bool isStaging;
    public bool useLocalhost;
}
