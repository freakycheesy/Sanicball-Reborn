using UnityEngine;
using FishNet.Object;
using SanicballCore;
using FishNet.Connection;
using Sanicball.Logic;
using System.ComponentModel;
using System;

public class LobbyScript : NetworkBehaviour
{
     public static LobbyScript Instance;

     public override void OnStartNetwork()
     {
          base.OnStartNetwork();
          Instance = this;
     }

     [ObserversRpc]
     public void ChangeSettingsRpc(MatchSettings settings)
     {
          MatchManager.Instance.CurrentSettings = settings;
          Debug.Log("Settings changed");
          MatchManager.Instance.MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
     }

     [ServerRpc(RequireOwnership = false)]
     public void ClientJoinRpc(NetworkConnection conn, string nickname)
     {
          MatchManager.Instance.clients.Add(new MatchClient(conn, nickname));
          Debug.Log("New client " + nickname);
     }

     [ServerRpc(RequireOwnership = false)]
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

     [ServerRpc(RequireOwnership = false)]
     public void PlayerLeaveRpc(NetworkConnection conn, ControlType type)
     {
          MatchManager.Instance.PlayerLeftCallback(conn, type);
     }

     [ServerRpc(RequireOwnership = false)]
     public void CharacterChangedRpc(NetworkConnection myGuid, ControlType ctrlType, int newCharacter)
     {
          MatchManager.Instance.CharacterChangedCallback(myGuid, ctrlType, newCharacter);
     }

     [ServerRpc(RequireOwnership = false)]
     public void ChangedReadyRpc(NetworkConnection myGuid, ControlType ctrlType, bool ready)
     {
          MatchManager.Instance.ChangedReadyCallback(myGuid, ctrlType, ready);
     }

     [ObserversRpc(ExcludeServer = false)]
     public void LoadRaceRpc()
     {
          MatchManager.Instance.LoadRaceCallback();
     }

     [ObserversRpc(ExcludeServer = false)]
     public void StartRaceRpc()
     {
          RaceManager.Instance.StartRaceCallback();
     }

     [ServerRpc(RequireOwnership = false)]
     public void LoadLobbyRpc()
     {
          MatchManager.Instance.LoadLobbyCallback();
     }

     [ServerRpc(RequireOwnership = false)]
     public void ChatMessageServerRpc(string from, string text)
     {
          ChatMessageRpc(from, text);
     }

     [ObserversRpc(ExcludeServer = false)]
     public void ChatMessageRpc(string from, string text)
     {
          MatchManager.Instance.ChatCallback(from, text);
     }

    [ServerRpc(RequireOwnership = false)]
    public void DoneRacingRpc(NetworkConnection clientGuid, ControlType ctrlType, double raceTimer, bool v)
    {
        RaceManager.Instance.DoneRacingCallback(clientGuid, ctrlType, raceTimer, v);
    }

     [ServerRpc(RequireOwnership = false)]
    public void CheckpointPassedMessage(NetworkConnection clientGuid, ControlType ctrlType, float lapTime)
    {
          RacePlayer.Instance.CheckpointPassedHandler(clientGuid, ctrlType, lapTime);
    }
}
