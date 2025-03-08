using BLMMX.Helpers;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    internal class CancelMatchCommand : AdminChatCommand
    {
        public override string CommandText => "cancelmatch";

        public override string Description => "取消对局";

        public override bool Execute(NetworkCommunicator executor, string args)
        {
            BLMMBehavior2.MarkCurrentMatchCancel();
            Helper.SendMessageToAllPeers("对局已取消");
            return true;
        }
    }
}
