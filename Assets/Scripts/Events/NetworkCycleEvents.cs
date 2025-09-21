using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UltEvents;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class NetworkCycleEvents : NetworkBehaviour
{
    [Header("Events")]

    public UltEvent OnValidateEvent;
    protected override void OnValidate() => OnValidateEvent?.Invoke();
    [Header("(DONT PUT DDOL INSIDE OF AWAKE, USE START INSTEAD)")]
    public UltEvent OnAwakeEvent;
    void Awake() => OnAwakeEvent?.Invoke();

    public UltEvent OnStartEvent;
    void Start() => OnStartEvent?.Invoke();
    [Header("Start/Stop Server Events")]
    public UltEvent OnStartServerEvent;
    public override void OnStartServer() => OnStartServerEvent?.Invoke();

    public UltEvent OnStopServerEvent;
    public override void OnStopServer() => OnStopServerEvent?.Invoke();
    [Header("Start/Stop Client Events")]
    public UltEvent OnStartClientEvent;
    public override void OnStartClient() => OnStartClientEvent?.Invoke();

    public UltEvent OnStopClientEvent;
    public override void OnStopClient() => OnStopClientEvent?.Invoke();

    [Header("Start/Stop Local Player Events")]
    public UltEvent OnStartLocalPlayerEvent;
    public override void OnStartLocalPlayer() => OnStartLocalPlayerEvent?.Invoke();

    public UltEvent OnStopLocalPlayerEvent;
    public override void OnStopLocalPlayer() => OnStopLocalPlayerEvent?.Invoke();
    [Header("Start/Stop Authority Events")]
    public UltEvent OnStartAuthorityEvent;
    public override void OnStartAuthority() => OnStartAuthorityEvent?.Invoke();

    public UltEvent OnStopAuthorityEvent;
    public override void OnStopAuthority() => OnStopAuthorityEvent?.Invoke();
}
