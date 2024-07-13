using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebClient;

[CreateAssetMenu(fileName = "Game", menuName = "Services/Game")]
public class GameServiceDriver : ServiceDriver
{
    public string dataKey = "data";
    public float botJoinTime = 1;
    public ushort serverPort = 3000;
    public ushort transportPort = 7777; //TODO: this should be generated via server later
    public WebClientConfig webConfig;
    public string networkAddress = "localhost";
}
