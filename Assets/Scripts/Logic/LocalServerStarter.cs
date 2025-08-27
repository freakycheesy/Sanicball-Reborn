using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanicball.Data;
using System.Threading;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Transporting;

public class LocalServerStarter : MonoBehaviour
{
	public void StartServer(string serverIp, ushort port = 7778)
	{
		Transport transport = NetworkManager.Instances[0].TransportManager.GetTransport(0);
		transport.SetServerBindAddress(serverIp, IPAddressType.IPv4);
		transport.SetPort(port);
		transport.StartConnection(true);
	}
}