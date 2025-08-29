using System;
using FishNet.Authenticating;
using FishNet.Connection;
using UnityEngine;

public class SanicAuth : Authenticator
{
    public override event Action<NetworkConnection, bool> OnAuthenticationResult;

    public override void OnRemoteConnection(NetworkConnection connection)
    {
        base.OnRemoteConnection(connection);
        OnAuthenticationResult?.Invoke(connection, true);
    }

}
