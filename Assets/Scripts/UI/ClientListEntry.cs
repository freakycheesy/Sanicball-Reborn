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

        public static List<ClientListEntry> ClientListEntries = new List<ClientListEntry>();
        public static ClientListEntry ClientListEntryPrefab = null;
        public static Transform ClientList;

        void OnEnable()
        {
            ClientListEntries.Add(this);
        }

        void OnDisable()
        {
            ClientListEntries.Remove(this);
        }

        public static void ClearEntries()
        {
            ClientListEntries.ForEach(ClearEntry);
        }

        public static void ClearEntry(ClientListEntry entry)
        {
            Destroy(entry.gameObject);
        }

        public static void AddEntries(MatchManager a, Transform b)
        {
            ClearEntries();
            ClientList = b;
            a.Clients.ForEach(c=>AddEntry(a, c));
        }

        public static void AddEntry(MatchManager a, MatchClient b)
        {
            ClientListEntry listEntry = Instantiate(ClientListEntryPrefab);
            listEntry.transform.SetParent(ClientList, false);

            listEntry.FillFields(b, a);
        }

        public void FillFields(MatchClient client, MatchManager manager)
        {
            nameField.text = client.Name;

            List<MatchPlayer> players = manager.Players.Where(a => a.CharacterId == client.ConnectionId).ToList();
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