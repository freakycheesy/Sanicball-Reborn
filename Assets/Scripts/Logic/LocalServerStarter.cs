using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanicball.Data;
using System.Threading;
using Sanicball.Logic;

public class LocalServerStarter : MonoBehaviour
{
	public void StartServer()
	{
		SanicNetworkManager.CreateLobby();
	}
}