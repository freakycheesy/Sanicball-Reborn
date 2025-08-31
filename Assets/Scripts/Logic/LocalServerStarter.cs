using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanicball.Data;
using System.Threading;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Sanicball.Logic;

public class LocalServerStarter : MonoBehaviour
{
	public void StartServer()
	{
		MatchManager.CreateLobby();
	}
}