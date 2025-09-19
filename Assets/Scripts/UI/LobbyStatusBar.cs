using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sanicball.Data;
using Sanicball.Logic;
using Telepathy;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class LobbyStatusBar : MonoBehaviour
    {
        [SerializeField]
        private Text leftText = null;
        [SerializeField]
        private Text rightText = null;

        [SerializeField]
        private RectTransform clientList = null;
        [SerializeField]
        private ClientListEntry clientListEntryPrefab = null;


        public static MatchManager manager;

        private void Awake()
        {
            manager ??= FindAnyObjectByType<MatchManager>();
            MatchManager.MatchManagerUpdated += Manager_Update;
            MatchManager.MatchManagerSpawned += (a, _) => OnStart(a);
        }

        private void Manager_Update(MatchManager a)
        {
            manager = a;
            UpdateText();
        }

        private void OnStart(MatchManager a)
        {
            manager = a;
            UpdateText();
        }

        private void UpdateText()
        {
            if (!manager) return;

            int clients = manager.Clients.Count;
            int players = manager.Players.Count;

            if (manager.AutoStartTimerOn)
            {
                leftText.text = "Match will start in " + GetTimeString(System.TimeSpan.FromSeconds(MatchManager.Instance.AutoStartTimer)) + ", or when all players are ready.";
            }
            else if (manager.Players.Count > 0)
            {
                leftText.text = "Match starts when all players are ready.";
            }
            else
            {
                leftText.text = "Match will not start without players.";
            }
            rightText.text = clients + " " + (clients != 1 ? "clients" : "client") + " - " + players + " " + (players != 1 ? "players" : "player");

            ClientListEntry.ClientListEntryPrefab = clientListEntryPrefab;
            ClientListEntry.ClearEntries();
            ClientListEntry.AddEntries(manager, clientList);
            
        }

        private string GetTimeString(System.TimeSpan timeToUse)
        {
            return string.Format("{0:00}:{1:00}", timeToUse.Minutes, timeToUse.Seconds);
        }
    }
}