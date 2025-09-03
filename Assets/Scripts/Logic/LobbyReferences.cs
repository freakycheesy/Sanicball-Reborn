using Mirror;
using Sanicball.Data;
using UnityEngine;

namespace Sanicball.Logic
{
    public class LobbyReferences : NetworkBehaviour
    {
        [SerializeField]
        private UI.LobbyStatusBar statusBar = null;

        [SerializeField]
        private UI.LocalPlayerManager localPlayerManager = null;

        [SerializeField]
        private UI.MatchSettingsPanel matchSettingsPanel = null;

        [SerializeField]
        private LobbyBallSpawner ballSpawner = null;

        [SerializeField]
        private UnityEngine.UI.Text countdownField = null;

        [SerializeField]
        private RectTransform markerContainer = null;

        public static LobbyReferences Active
        {
            get; private set;
        }

        public UI.LobbyStatusBar StatusBar { get { return statusBar; } }
        public UI.LocalPlayerManager LocalPlayerManager { get { return localPlayerManager; } }
        public UI.MatchSettingsPanel MatchSettingsPanel { get { return matchSettingsPanel; } }
        public LobbyBallSpawner BallSpawner { get { return ballSpawner; } }
        public UnityEngine.UI.Text CountdownField { get { return countdownField; } }
        public RectTransform MarkerContainer { get { return markerContainer; } }
        public MatchManager MatchManagerPrefab;
        private void Start()
        {
            Active = this;
            CameraFade.StartAlphaFade(Color.black, true, 1f);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            var manager = Instantiate(MatchManagerPrefab);
            DontDestroyOnLoad(manager.gameObject);
            NetworkServer.Spawn(manager.gameObject);
            manager.currentSettings = ActiveData.MatchSettings;
            manager.showSettingsOnLobbyLoad = true;
            //manager.GoToLobby();
            manager.activeChat = Instantiate(manager.chatPrefab);
            manager.activeChat.MessageSent += manager.LocalChatMessageSent;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isClientOnly) MatchManager.Instance.showSettingsOnLobbyLoad = false;
        }

        public override void OnStopServer()
        {
            if (MatchManager.Instance)
            {
                MatchManager.Instance.inLobby = false;
                MatchManager.Instance.loadingLobby = false;
                NetworkServer.Destroy(MatchManager.Instance.gameObject);
            }
        }
    }
}
