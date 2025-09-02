using UnityEngine;
using SanicballCore;
using Sanicball.Logic;
using System;
using Sanicball.Data;
using Mirror;

public class LobbyScript : NetworkBehaviour
{
     public static LobbyScript Instance;

     public override void OnStartClient()
     {
          base.OnStartClient();
          if (Instance)
          {
               NetworkServer.Destroy(netIdentity.gameObject);
               Destroy(this.gameObject);
               return;
          }
          Instance = this;
          MatchManager.Instance.myGuid = Instance.connectionToClient;
          ClientJoinRpc(MatchManager.Instance.myGuid, ActiveData.GameSettings.nickname);
     }

     [ClientRpc(includeOwner = true)]
     public void ChangeSettingsRpc(MatchSettings settings)
     {
          MatchManager.Instance.CurrentSettings = settings;
          Debug.Log("Settings changed");
          MatchManager.Instance.MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
     }

     [Command(requiresAuthority = false)]
     public void ClientJoinRpc(NetworkConnectionToClient conn, string nickname)
     {
          var matchClient = new MatchClient(conn, nickname);
          if (MatchManager.Instance.Clients.Contains(matchClient)) return;
          MatchManager.Instance.clients.Add(matchClient);
          Debug.Log("New client " + nickname);
     }

     [Command(requiresAuthority = false)]
     public void PlayerJoinRpc(NetworkConnectionToClient conn, ControlType type, int characterId)
     {
          var p = new MatchPlayer(conn, type, characterId);
          MatchManager.Instance.players.Add(p);

          if (MatchManager.Instance.inLobby)
          {
               MatchManager.Instance.SpawnLobbyBall(p);
          }

          MatchManager.Instance.StopLobbyTimer();

          MatchManager.Instance.MatchPlayerAdded(this, new MatchPlayerEventArgs(p, conn == MatchManager.Instance.myGuid));
     }

     [Command(requiresAuthority = false)]
     public void PlayerLeaveRpc(NetworkConnectionToClient conn, ControlType type)
     {
          MatchManager.Instance.PlayerLeftCallback(conn, type);
     }

     [Command(requiresAuthority = false)]
     public void CharacterChangedRpc(NetworkConnectionToClient myGuid, ControlType ctrlType, int newCharacter)
     {
          MatchManager.Instance.CharacterChangedCallback(myGuid, ctrlType, newCharacter);
     }

     [Command(requiresAuthority = false)]
     public void ChangedReadyServerRpc(NetworkConnectionToClient myGuid, ControlType ctrlType, bool ready)
    {
          ChangedReadyRpc(myGuid, ctrlType, ready);
    }

     [Command(requiresAuthority = false)]
     private void ChangedReadyRpc(NetworkConnectionToClient myGuid, ControlType ctrlType, bool ready)
     {
          MatchManager.Instance.ChangedReadyCallback(myGuid, ctrlType);
     }

     [Command(requiresAuthority = false)]
     public void LoadRaceRpc()
     {
          MatchManager.Instance.LoadRaceCallback();
     }

     [Command(requiresAuthority = false)]
     public void ReadyUpRaceRpc(NetworkConnectionToClient myGuid, ControlType ctrlType)
     {
          Debug.Log("Ready Up");
          RaceManager.Instance.ReadyUpRace(myGuid, ctrlType);
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
