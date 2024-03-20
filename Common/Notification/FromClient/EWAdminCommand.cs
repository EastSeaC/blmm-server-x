using System;
using System.Collections.Generic;
using System.Text;
using BLMMX.Common.Notification;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace BLMMX.Common.Notification.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    internal class EWAdminCommand : GameNetworkMessage
    {
        private EWNotificationType _notificationType;
        private string _message;

        public string Message { get => _message; set => _message = value; }
        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Messaging;
        }

        protected override string OnGetLogFormat()
        {
            return "EW Notification message from server";
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            _notificationType = (EWNotificationType)ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
            _message = ReadStringFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket((int)_notificationType, CompressionBasic.DebugIntNonCompressionInfo);
            WriteStringToPacket(_message);
        }
    }
}
