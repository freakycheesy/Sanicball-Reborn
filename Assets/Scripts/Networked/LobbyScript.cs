using UnityEngine;
using FishNet.Object;
using SanicballCore;
using FishNet.Connection;
using Sanicball.Logic;
using System.ComponentModel;
using System;
using GameKit.Dependencies.Utilities.Types;
using Sanicball.Data;
using System.Collections;

public class LobbyScript : NetworkBehaviour
{
     public static LobbyScript Instance;

    public override void OnStartNetwork()
    {
          base.OnStartNetwork();
          if (Instance)
          {
               Despawn(NetworkObject, DespawnType.Destroy);
               Destroy(this.gameObject);
               return;
          }
          Instance = this;
     }

     public override void OnStartClient()
     {
          base.OnStartClient();
          MatchManager.Instance.myGuid = Instance.LocalConnection;
          ClientJoinRpc(MatchManager.Instance.myGuid, ActiveData.GameSettings.nickname);
     }


     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ChangeSettingsRpc(MatchSettings settings)
     {
          MatchManager.Instance.CurrentSettings = settings;
          Debug.Log("Settings changed");
          MatchManager.Instance.MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ClientJoinRpc(NetworkConnection conn, string nickname)
     {
          var matchClient = new MatchClient(conn, nickname);
          if (MatchManager.Instance.Clients.Contains(matchClient)) return;
          MatchManager.Instance.clients.Add(matchClient);
          Debug.Log("New client " + nickname);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void PlayerJoinRpc(NetworkConnection conn, ControlType type, int characterId)
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

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void PlayerLeaveRpc(NetworkConnection conn, ControlType type)
     {
          MatchManager.Instance.PlayerLeftCallback(conn, type);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void CharacterChangedRpc(NetworkConnection myGuid, ControlType ctrlType, int newCharacter)
     {
          MatchManager.Instance.CharacterChangedCallback(myGuid, ctrlType, newCharacter);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ChangedReadyServerRpc(NetworkConnection myGuid, ControlType ctrlType, bool ready)
    {
          ChangedReadyRpc(myGuid, ctrlType, ready);
    }

     [ObserversRpc(ExcludeServer = false)]
     private void ChangedReadyRpc(NetworkConnection myGuid, ControlType ctrlType, bool ready)
     {
          MatchManager.Instance.ChangedReadyCallback(myGuid, ctrlType);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void LoadRaceRpc()
     {
          MatchManager.Instance.LoadRaceCallback();
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ReadyUpRaceRpc(NetworkConnection myGuid, ControlType ctrlType)
     {
          Debug.Log("Ready Up");
          RaceManager.Instance.ReadyUpRace(myGuid, ctrlType);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void LoadLobbyRpc()
     {
          MatchManager.Instance.LoadLobbyCallback();
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ChatMessageServerRpc(string from, string text)
     {
          ChatMessageRpc(from, text);
     }

     [ServerRpc(RequireOwnership = false, RunLocally = true)]
     public void ChatMessageRpc(string from, string text)
     {
          MatchManager.Instance.ChatCallback(from, text);
     }

     [TargetRpc]
     public void DoneRacingRpc(NetworkConnection clientGuid, ControlType ctrlType, double raceTimer, bool v)
     {
          RaceManager.Instance.DoneRacingCallback(clientGuid, ctrlType, raceTimer, v);
     }

     [TargetRpc]
     public void CheckpointPassedMessage(NetworkConnection clientGuid, ControlType ctrlType, float lapTime)
     {
          RacePlayer.Instance.CheckpointPassedHandler(clientGuid, ctrlType, lapTime);
     }
}
