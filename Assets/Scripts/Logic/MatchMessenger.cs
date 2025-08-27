using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Broadcast;
using FishNet.Transporting;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public abstract class MatchMessenger
    {
        /// <summary>
        /// Sends a message to this messenger.
        /// </summary>
        /// <param name="message"></param>
        public abstract void SendMessage<T>(T message) where T : struct, IBroadcast;

        public abstract void UpdateListeners();

        public abstract void Close();

        public abstract void RegisterBroadcast<T>(Action<T, Channel> handler) where T : struct, IBroadcast;

        public abstract void RemoveListener<T>(Action<T, Channel> startRaceCallback) where T : struct, IBroadcast;
    }
}