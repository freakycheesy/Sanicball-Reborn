using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class ServerListItem : MonoBehaviour
    {
        [SerializeField]
        private Text serverNameText = null;
        [SerializeField]
        private Text serverStatusText = null;
        [SerializeField]
        private Text playerCountText = null;
        [SerializeField]
        private Text pingText = null;

        private ZaLobbyInfo info;

        public void Init(ZaLobbyInfo info, System.Net.IPEndPoint endpoint, int pingMs, bool isLocal)
        {
            serverNameText.text = info.Name;
            serverStatusText.text = info.InRace ? "In race" : "In lobby";
            if (isLocal)
            {
                serverStatusText.text += " - LAN server";
            }
            playerCountText.text = info.Players + "/" + info.MaxPlayers;
            pingText.text = pingMs + "ms";
            info.IP = endpoint.Address.ToString();
        }

        public void Join()
        {
            MatchStarter starter = MatchStarter.Instance;
            if (starter)
            {
                starter.JoinOnlineGame(info.IP);
            }
            else
            {
                Debug.LogError("No match starter found");
            }
        }
    }
}
