﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SanicballCore.Server;
using Sanicball.Data;
using System.Threading;

public class LocalServerStarter : MonoBehaviour
{

	private Thread serverThread;
	private static CommandQueue commandQueue = new CommandQueue();

	public void InitServer(int port, int maxPlayers, string serverName, bool showOnList, string serverListURL)
	{
		if (serverThread == null)
		{
			serverThread = new Thread(param =>
			{
				string matchSettingsPath = (string)param;
				StartServer(port, maxPlayers, serverName, matchSettingsPath, showOnList, serverListURL);
			});
			serverThread.Start(Application.persistentDataPath + "/MatchSettings.json");
		}
	}

	private void Update()
	{
		if (!serverThread.IsAlive)
		{
			Destroy(gameObject);
		}
	}

	public void OnDestroy()
	{
		serverThread.Abort();
		serverThread.Join();
	}

	private void StartServer(int port, int maxPlayers, string serverName, string matchSettingsPath, bool showOnList, string serverListURL)
	{
		Server serv = new Server(commandQueue, true);
		Debug.Log("Starting Server");
		serv.Start(port, maxPlayers, serverName, ActiveData.GameSettings.nickname, matchSettingsPath, showOnList, serverListURL);
		Debug.Log("Stopping Server");
		serv.Dispose();
	}
}