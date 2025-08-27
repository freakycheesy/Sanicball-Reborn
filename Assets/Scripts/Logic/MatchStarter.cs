using FishNet.Authenticating;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Client;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public class MatchStarter : MonoBehaviour
    {
        public const string APP_ID = "Sanicball";

        [SerializeField]
        private MatchManager matchManagerPrefab = null;
        [SerializeField]
        private UI.Popup connectingPopupPrefab = null;
        [SerializeField]
        private UI.PopupHandler popupHandler = null;

        private UI.PopupConnecting activeConnectingPopup;

        //NetClient for when joining online matches
        private ClientManager joiningClient => NetworkManager.Instances[0].ClientManager;
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
                joiningClient.Connection.Disconnect(true);
            }
        }

        public void BeginLocalGame()
        {
            MatchManager manager = Instantiate(matchManagerPrefab);
            manager.InitLocalMatch();
        }

        public void JoinOnlineGame(string ip = "127.0.0.1", ushort port = 25000)
        {
            JoinOnlineGame(new(ip, port));
        }

        public void JoinOnlineGame(ZaLobbyInfo lobbyInfo)
        {
            ClientInfo info = new ClientInfo(GameVersion.AS_FLOAT, GameVersion.IS_TESTING);
            MatchManager.JoinLobby(lobbyInfo.IP, lobbyInfo.Port);
            joiningClient.StartConnection();

            popupHandler.OpenPopup(connectingPopupPrefab);

            activeConnectingPopup = PopupConnecting.Instance;
        }

        //Called when succesfully connected to a server
        private void BeginOnlineGame(MatchState matchState)
        {
            MatchManager manager = Instantiate(matchManagerPrefab);
            manager.InitOnlineMatch(matchState);
        }
    }
}