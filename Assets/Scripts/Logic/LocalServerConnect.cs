using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanicball.UI;
using Sanicball.Logic;
using System.Net.Sockets;

public class LocalServerConnect : MonoBehaviour {
	
	[SerializeField]
	private PopupHandler popupHandler = null;
	
	public void Start(){
		popupHandler = PopupHandler.Instance;
	}

	public void Connect(string serverIp, ushort port = 25000){
		MatchManager.JoinLobby(serverIp);
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
