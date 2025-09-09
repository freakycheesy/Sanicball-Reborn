using System;
using Mirror;
using Sanicball.Data;
using Sanicball.Logic;
using Sanicball.UI;
using SanicballCore.MatchMessages;
using UnityEngine;

public class SanicNetworkManager : AddressablesNetworkManager
{
    private new static SanicNetworkManager singleton { get; set; }
    public static SanicNetworkManager Singleton => singleton;
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
        if(singleton.isNetworkActive) singleton?.StopHost();
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        if(MatchManager.Instance) Destroy(MatchManager.Instance.gameObject);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if(Chat.Instance) Destroy(Chat.Instance.gameObject);
    }


    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        singleton = this;
        if (singleton != this) singleton = this;
    }

}
