using Mirror;
using Sanicball.Logic;
using UnityEngine;

public class SanicNetworkManager : AddressableNetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        MatchManager.Instance.showSettingsOnLobbyLoad = true;
        MatchManager.Instance.GoToLobby();
        MatchManager.Instance.activeChat = Instantiate(MatchManager.Instance.chatPrefab);
        MatchManager.Instance.activeChat.MessageSent += MatchManager.Instance.LocalChatMessageSent;
        NetworkServer.Spawn(Instantiate(MatchManager.Instance.LobbyPrefab.gameObject));
    }

    public override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
