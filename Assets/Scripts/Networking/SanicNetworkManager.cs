using System;
using Mirror;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore.MatchMessages;
using UnityEngine;

public class SanicNetworkManager : AddressablesNetworkManager
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
        try
        {
            singleton?.StartHost();
        }
        catch (Exception e)
        {
            Debug.Log($"Trying to Join Client, Reason: {e}");
            singleton?.StartClient();
        }
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
