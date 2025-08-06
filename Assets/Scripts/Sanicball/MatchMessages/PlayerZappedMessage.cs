﻿namespace SanicballCore.MatchMessages
{
    public class PlayerZappedMessage : MatchMessage
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }

        public PlayerZappedMessage(System.Guid clientGuid, ControlType ctrlType)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
        }
    }
}