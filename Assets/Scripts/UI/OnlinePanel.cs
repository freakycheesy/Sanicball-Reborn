using System;
using System.Collections.Generic;
using System.Net;
using FishNet.Managing;
using FishNet.Transporting;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class OnlinePanel : MonoBehaviour
    {
        public Transform targetServerListContainer;
        public Text errorField;
        public Text serverCountField;
        public ServerListItem serverListItemPrefab;
        public Selectable aboveList;
        public Selectable belowList;

        private List<ServerListItem> servers = new List<ServerListItem>();

        //Stores server browser IPs, so they can be differentiated from LAN servers
        private List<string> serverBrowserIPs = new List<string>();

        private FishNet.Discovery.NetworkDiscovery discoveryClient;
        private UnityWebRequest serverBrowserRequester;
        private DateTime latestLocalRefreshTime;
        private DateTime latestBrowserRefreshTime;

        public void RefreshServers()
        {
            serverBrowserIPs.Clear();
            discoveryClient.SearchForServers();

			//serverBrowserRequester = new UnityWebRequest(ActiveData.GameSettings.serverListURL);

            serverCountField.text = "Refreshing servers, hang on...";
            errorField.enabled = false;

            //Clear old servers
            foreach (var serv in servers)
            {
                Destroy(serv.gameObject);
            }
            servers.Clear();
        }

        private void Awake()
        {
            errorField.enabled = false;
            discoveryClient = FindAnyObjectByType<FishNet.Discovery.NetworkDiscovery>();
            discoveryClient.SearchForServers();

            discoveryClient.ServerFoundCallback += (msg) =>
            {
                ZaLobbyInfo lobbyInfo = new ZaLobbyInfo();
                try
                {
                    lobbyInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ZaLobbyInfo>(msg.ToString());
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    Debug.LogError("Failed to deserialize info for a server: " + ex.Message);
                }

                //double timeDiff = (DateTime.UtcNow - info.Timestamp).TotalMilliseconds;
                bool isLocal = !serverBrowserIPs.Contains(msg.Address.ToString());

                DateTime timeToCompareTo = isLocal ? latestLocalRefreshTime : latestBrowserRefreshTime;
                double timeDiff = (DateTime.Now - timeToCompareTo).TotalMilliseconds;

                var server = Instantiate(serverListItemPrefab);
                server.transform.SetParent(targetServerListContainer, false);
                server.Init(lobbyInfo, msg, (int)timeDiff, isLocal);
                servers.Add(server);
                RefreshNavigation();

                serverCountField.text = servers.Count + (servers.Count == 1 ? " server" : " servers");
            }; //Hack to make sure the callback is not null

            NetworkManager.Instances[0].ServerManager.OnServerConnectionState += StopDiscovery;

            NetworkManager.Instances[0].ClientManager.OnClientConnectionState += StopDiscovery;
        }

        void OnDestroy()
        {
            NetworkManager.Instances[0].ServerManager.OnServerConnectionState -= StopDiscovery;

            NetworkManager.Instances[0].ClientManager.OnClientConnectionState -= StopDiscovery;
        }

        private void StopDiscovery(ServerConnectionStateArgs state)
        {
            discoveryClient.StopSearchingOrAdvertising();
        }

        private void StopDiscovery(ClientConnectionStateArgs state)
        {
            discoveryClient.StopSearchingOrAdvertising();
        }

        private void Update()
        {
            //Refresh on f5 (pretty nifty)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                RefreshServers();
            }

            //Check for response from the server browser requester
            if (serverBrowserRequester != null && serverBrowserRequester.isDone)
            {
                if (string.IsNullOrEmpty(serverBrowserRequester.error))
                {
                    serverCountField.text = "0 servers";
                }
                else
                {
                    Debug.LogError("Failed to receive servers - " + serverBrowserRequester.error);
                    serverCountField.text = "Cannot access server list URL!";
                }

                serverBrowserRequester = null;
            }
        }

        private void RefreshNavigation()
        {
            for (var i = 0; i < servers.Count; i++)
            {
                var button = servers[i].GetComponent<Button>();
                if (button)
                {
                    var nav = new Navigation() { mode = Navigation.Mode.Explicit };
                    //Up navigation
                    if (i == 0)
                    {
                        nav.selectOnUp = aboveList;
                        var nav2 = aboveList.navigation;
                        nav2.selectOnDown = button;
                        aboveList.navigation = nav2;
                    }
                    else
                    {
                        nav.selectOnUp = servers[i - 1].GetComponent<Button>();
                    }
                    //Down navigation
                    if (i == servers.Count - 1)
                    {
                        nav.selectOnDown = belowList;
                        var nav2 = belowList.navigation;
                        nav2.selectOnUp = button;
                        belowList.navigation = nav2;
                    }
                    else
                    {
                        nav.selectOnDown = servers[i + 1].GetComponent<Button>();
                    }

                    button.navigation = nav;
                }
            }
        }
    }
}
