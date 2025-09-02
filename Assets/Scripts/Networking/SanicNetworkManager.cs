using Mirror;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore.MatchMessages;
using UnityEngine;

public class SanicNetworkManager : AddressableNetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        MatchManager.Instance.currentSettings = ActiveData.MatchSettings;
        MatchManager.Instance.showSettingsOnLobbyLoad = true;
        MatchManager.Instance.GoToLobby();
        MatchManager.Instance.activeChat = Instantiate(MatchManager.Instance.chatPrefab);
        MatchManager.Instance.activeChat.MessageSent += MatchManager.Instance.LocalChatMessageSent;
        //NetworkServer.Spawn(Instantiate(MatchManager.Instance.LobbyPrefab.gameObject));
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        MatchManager.Instance.showSettingsOnLobbyLoad = false;

        NetworkClient.Send<ClientJoinedMessage>(new(NetworkServer.localConnection.connectionId, ActiveData.GameSettings.nickname));
    }

    public override void OnStopHost()
    {
        if (MatchManager.Instance)
        {
            MatchManager.Instance.inLobby = false;
            MatchManager.Instance.loadingLobby = false;
            Destroy(MatchManager.Instance.gameObject);
        }
        base.OnStopHost();
    }

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

}
