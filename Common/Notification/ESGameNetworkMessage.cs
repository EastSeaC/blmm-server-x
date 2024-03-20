using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace BLMMX.Common.Notification
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    internal class ESGameNetworkMessage : GameNetworkMessage
    {
        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            throw new NotImplementedException();
        }

        protected override string OnGetLogFormat()
        {
            throw new NotImplementedException();
        }

        protected override bool OnRead()
        {
            throw new NotImplementedException();
        }

        protected override void OnWrite()
        {

        }
    }
}