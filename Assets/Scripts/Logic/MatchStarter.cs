using Mirror;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public class MatchStarter : MonoBehaviour
    {
        public const string APP_ID = "Sanicball";

        public MatchManager matchManagerPrefab = null;
        public UI.Popup connectingPopupPrefab = null;
        public UI.PopupHandler popupHandler = null;

        public UI.PopupConnecting activeConnectingPopup;

        //NetClient for when joining online matches
        public static MatchStarter Instance;
        void Start()
        {
            Instance = this;
            activeConnectingPopup = PopupConnecting.Instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                popupHandler.CloseActivePopup();
                NetworkManager.singleton.StopHost();
            }
        }

        public void BeginLocalGame()
        {
            SanicNetworkManager.CreateLobby();
        }

        public void JoinOnlineGame(string ip = "127.0.0.1", ushort port = 25000)
        {
            JoinOnlineGame(new(ip, port));
        }

        public void JoinOnlineGame(ZaLobbyInfo lobbyInfo)
        {
            SanicNetworkManager.JoinLobby(lobbyInfo.IP);

            popupHandler.OpenPopup(connectingPopupPrefab);

            activeConnectingPopup = PopupConnecting.Instance;
        }

        //Called when succesfully connected to a server
        private void BeginOnlineGame(MatchState matchState)
        {
            //MatchManager manager = Instantiate(matchManagerPrefab);
            SanicNetworkManager.CreateLobby();
        }
    }
}