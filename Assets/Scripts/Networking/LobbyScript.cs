/*using UnityEngine;
using SanicballCore;
using Sanicball.Logic;
using System;
using Sanicball.Data;
using Mirror;
using System.Linq;

public class LobbyScript : NetworkBehaviour
{
     public static LobbyScript Instance;

     public override void OnStartClient()
     {
          if (Instance != this) return;
          base.OnStartClient();
          MatchManager.Instance.myGuid = Instance.connectionToClient;
          ClientJoinRpc(ActiveData.GameSettings.nickname);
     }

    void Start()
    {
          if (Instance)
          {
               NetworkServer.Destroy(netIdentity.gameObject);
               Destroy(this.gameObject);
               return;
          }
          DontDestroyOnLoad(this.gameObject);
          Instance = this;
     }

    [ClientRpc(includeOwner = true)]
     public void ChangeSettingsRpc(MatchSettings settings)
     {
          MatchManager.Instance.CurrentSettings = settings;
          Debug.Log("Settings changed");
          MatchManager.Instance.MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
     }

     [Command(requiresAuthority = false)]
     public void ClientJoinRpc(string nickname, NetworkConnectionToClient sender = null)
     {
          var matchClient = new MatchClient(sender, nickname);
          if (MatchManager.Instance.Clients.Contains(matchClient)) return;
          MatchManager.Instance.clients.Add(matchClient);
          Debug.Log("New client " + nickname);
     }

     [Command(requiresAuthority = false)]
     public void PlayerJoinRpc(ControlType type, int characterId, NetworkConnectionToClient sender = null)
     {
          var p = new MatchPlayer(sender, type, characterId);
          MatchManager.Instance.players.Add(p);

          if (MatchManager.Instance.inLobby)
          {
               MatchManager.Instance.SpawnLobbyBall(p);
          }

          MatchManager.Instance.StopLobbyTimer();

          MatchManager.Instance.MatchPlayerAdded(this, new MatchPlayerEventArgs(p, sender == MatchManager.Instance.myGuid));
     }

     [Command(requiresAuthority = false)]
     public void PlayerLeaveRpc(ControlType type, NetworkConnectionToClient sender = null)
     {
          MatchManager.Instance.PlayerLeftCallback(sender, type);
     }

     [Command(requiresAuthority = false)]
     public void CharacterChangedRpc(ControlType ctrlType, int newCharacter, NetworkConnectionToClient sender = null)
     {
          MatchManager.Instance.CharacterChangedCallback(sender, ctrlType, newCharacter);
     }

     [Command(requiresAuthority = false)]
     public void ChangedReadyServerRpc(ControlType ctrlType, bool ready, NetworkConnectionToClient sender = null)
     {
          var player = MatchManager.Instance.players.FirstOrDefault(a => a.ClientGuid == sender && a.CtrlType == ctrlType);
          if(player != null) ChangedReadyRpc(player.ClientGuid.connectionId, player.CtrlType);
     }

     [ClientRpc]
     private void ChangedReadyRpc(int connectionId, ControlType ctrlType)
     {
          var player = MatchManager.Instance.players.FirstOrDefault(a => a.ClientGuid.connectionId == connectionId && a.CtrlType == ctrlType);
          MatchManager.Instance.ChangedReadyCallback(player);
     }

     [Command(requiresAuthority = false)]
     public void LoadRaceRpc()
     {
          MatchManager.Instance.LoadRaceCallback();
     }

     [Command(requiresAuthority = false)]
     public void ReadyUpRaceRpc(ControlType ctrlType, NetworkConnectionToClient sender = null)
     {
          Debug.Log("Ready Up");
          RaceManager.Instance.ReadyUpRace(sender, ctrlType);
     }

     [Command(requiresAuthority = false)]
     public void LoadLobbyRpc()
     {
          MatchManager.Instance.LoadLobbyCallback();
     }

     [Command(requiresAuthority = false)]
     public void ChatMessageServerRpc(string from, string text)
     {
          ChatMessageRpc(from, text);
     }

     [Command(requiresAuthority = false)]
     public void ChatMessageRpc(string from, string text)
     {
          MatchManager.Instance.ChatCallback(from, text);
     }

     [TargetRpc]
     public void DoneRacingRpc(NetworkConnectionToClient clientGuid, ControlType ctrlType, double raceTimer, bool v)
     {
          RaceManager.Instance.DoneRacingCallback(clientGuid, ctrlType, raceTimer, v);
     }

     [TargetRpc]
     public void CheckpointPassedMessage(NetworkConnectionToClient clientGuid, ControlType ctrlType, float lapTime)
     {
          RacePlayer.Instance.CheckpointPassedHandler(clientGuid, ctrlType, lapTime);
     }
}
*/