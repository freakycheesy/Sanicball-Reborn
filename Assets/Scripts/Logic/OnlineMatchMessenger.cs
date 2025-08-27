using System;
using System.Collections;
using System.Reflection;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Transporting;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public class DisconnectArgs : EventArgs
    {
        public string Reason { get; private set; }

        public DisconnectArgs(string reason)
        {
            Reason = reason;
        }
    }

    public class PlayerMovementArgs : EventArgs
    {
        public double Timestamp { get; private set; }
        public PlayerMovement Movement { get; private set; }

        public PlayerMovementArgs(double timestamp, PlayerMovement movement)
        {
            Timestamp = timestamp;
            Movement = movement;
        }
    }

    public class OnlineMatchMessenger : MatchMessenger
    {
        public const string APP_ID = "Sanicball";

        private ClientManager client;

        //Settings to use for both serializing and deserializing messages
        private Newtonsoft.Json.JsonSerializerSettings serializerSettings;

        public event EventHandler<PlayerMovementArgs> OnPlayerMovement;
        public event EventHandler<DisconnectArgs> Disconnected;

        public OnlineMatchMessenger(ClientManager client)
        {
            Debug.Log("Using OnlineMatchMessenger");
            this.client = client;

            serializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            serializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        }

        public override void SendMessage<T>(T message)
        {
            NetworkManager.Instances[0].ClientManager.Broadcast(message, FishNet.Transporting.Channel.Reliable);
        }

        public void SendPlayerMovement(MatchPlayer player)
        {
            var msg = PlayerMovement.CreateFromPlayer(player);
            client.Broadcast(msg, FishNet.Transporting.Channel.Unreliable);
        }

        public override void Close()
        {
            client.Connection.Disconnect(true);
        }

        public override void UpdateListeners()
        {
        }

        public override void RegisterBroadcast<T>(Action<T, Channel> handler)
        {
            NetworkManager.Instances[0].ClientManager.RegisterBroadcast(handler);
        }

        public override void RemoveListener<T>(Action<T, Channel> handler)
        {
            NetworkManager.Instances[0].ClientManager.UnregisterBroadcast(handler);
        }
    }
}