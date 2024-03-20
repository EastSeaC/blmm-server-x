using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace BLMMX.Common.Notification;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class EWNotification : GameNetworkMessage
{
    public EWNotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public string SoundEvent { get; set; } = string.Empty;

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
        Type = (EWNotificationType)ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
        Message = ReadStringFromPacket(ref bufferReadValid);
        SoundEvent = ReadStringFromPacket(ref bufferReadValid);
        return bufferReadValid;
    }

    protected override void OnWrite()
    {
        WriteIntToPacket((int)Type, CompressionBasic.DebugIntNonCompressionInfo);
        WriteStringToPacket(Message);
        WriteStringToPacket(SoundEvent);
    }
}
