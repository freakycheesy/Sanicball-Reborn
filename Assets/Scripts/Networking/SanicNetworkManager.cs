using Mirror;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore.MatchMessages;
using UnityEngine;

public class SanicNetworkManager : AddressableNetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public static void CreateLobby()
    {
        singleton?.StartHost();
    }

    public static void JoinLobby(string ip)
    {
        singleton.networkAddress = ip;
        singleton?.StartClient();
    }

    public static void LeaveLobby()
    {
        singleton?.StopHost();
    }

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }


}
