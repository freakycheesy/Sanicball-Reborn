using System.Collections.Generic;
using Sanicball.Data;
using Sanicball.Logic;
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

        private List<ClientListEntry> curClientListEntries = new List<ClientListEntry>();

        private void Awake()
        {
            MatchManager.MatchManagerSpawned += (_, _) => OnStart();
        }
        private void OnStart()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (!MatchManager.Instance) return;

            int clients = MatchManager.Instance.Clients.Count;
            int players = MatchManager.Instance.Players.Count;

            if (MatchManager.Instance.AutoStartTimerOn)
            {
                leftText.text = "Match will start in " + GetTimeString(System.TimeSpan.FromSeconds(MatchManager.Instance.AutoStartTimer)) + ", or when all players are ready.";
            }
            else if (MatchManager.Instance.Players.Count > 0)
            {
                leftText.text = "Match starts when all players are ready.";
            }
            else
            {
                leftText.text = "Match will not start without players.";
            }
            rightText.text = clients + " " + (clients != 1 ? "clients" : "client") + " - " + players + " " + (players != 1 ? "players" : "player");

            foreach (ClientListEntry entry in curClientListEntries)
            {
                Destroy(entry.gameObject);
            }
            curClientListEntries.Clear();

            foreach (MatchClient c in MatchManager.Instance.Clients)
            {
                ClientListEntry listEntry = Instantiate(clientListEntryPrefab);
                listEntry.transform.SetParent(clientList, false);

                listEntry.FillFields(c, MatchManager.Instance);
                curClientListEntries.Add(listEntry);
            }
        }

        private void Update()
        {
            UpdateText();
        }

        private string GetTimeString(System.TimeSpan timeToUse)
        {
            return string.Format("{0:00}:{1:00}", timeToUse.Minutes, timeToUse.Seconds);
        }
    }
}