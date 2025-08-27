using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanicball.UI;
using Sanicball.Logic;
using System.Net.Sockets;
using FishNet.Managing;
using FishNet.Transporting;

public class LocalServerConnect : MonoBehaviour {
	
	[SerializeField]
	private PopupHandler popupHandler = null;
	
	public void Start(){
		popupHandler = PopupHandler.Instance;
	}

	public void Connect(string serverIp, ushort port = 7778){
		Transport transport = NetworkManager.Instances[0].TransportManager.GetTransport(0);
		transport.SetClientAddress(serverIp);
		transport.SetPort(port);
		transport.StartConnection(false);
	}

	private bool Ping(string ip, int port){
		try{
			using(var client = new UdpClient()){
				client.Connect(ip,port);
				Debug.Log("[PopupCreateServer.cs -> Ping] "+ip+":"+port+" succeeded.");
				return true;
			}
		}catch(SocketException e){
			Debug.LogError(e);
			return false;
		}
	}
}
