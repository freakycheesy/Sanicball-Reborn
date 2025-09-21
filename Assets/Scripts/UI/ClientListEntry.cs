using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class ClientListEntry : MonoBehaviour
    {
        [SerializeField]
        private Text nameField = null;
        [SerializeField]
        private Text playerCountField = null;

        public static ClientListEntry ClientListEntryPrefab = null;
        public static Transform ClientList;

        void OnEnable()
        {
            LobbyStatusBar.Instance.ClientListEntries.Add(this);
        }

        void OnDisable()
        {
            LobbyStatusBar.Instance.ClientListEntries.Remove(this);
        }

        public static void ClearEntries()
        {
            foreach (var entry in LobbyStatusBar.Instance.ClientListEntries.ToArray()) ClearEntry(entry);
        }

        public static void ClearEntry(ClientListEntry entry)
        {
            Destroy(entry.gameObject);
        }

        public static void AddEntries(MatchManager a, Transform b)
        {
            ClearEntries();
            ClientList = b;
            MatchClient.Clients.ToList().ForEach(c=>AddEntry(a, c));
        }

        public static void AddEntry(MatchManager a, MatchClient b)
        {
            ClientListEntry listEntry = Instantiate(ClientListEntryPrefab);
            listEntry.transform.SetParent(ClientList, false);

            listEntry.FillFields(b, a);
        }

        public void FillFields(MatchClient client, MatchManager manager)
        {
            nameField.text = client.Nickname;

            List<MatchPlayer> players = manager.Players.Where(a => a.ConnectionId == client.GetConnection()).ToList();
            int playersTotal = players.Count();
            int playersReady = players.Count(a => a.ReadyToRace);

            if (playersTotal == 0)
            {
                playerCountField.text = "Spectating";
            }
            else
            {
                playerCountField.text = playersReady + "/" + playersTotal + " ready";
            }
        }
    }
}