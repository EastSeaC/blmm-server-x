using BLMMX.Helpers;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    public class PlayerInfoCommand : AdminChatCommand
    {
        public override string CommandText => "playerinfo";
        public override string Description => "Prints information about all players whose names contain the given NAME";

        public override bool Execute(NetworkCommunicator executor, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Helper.SendMessageToPeer(executor, "Please provide a player name.");
                return false;
            }

            bool foundAny = false;
            foreach (var peer in GameNetwork.NetworkPeers)
            {
                if (peer.UserName.Contains(args))
                {
                    Helper.SendMessageToPeer(executor, $"{peer.UserName}'s PlayerID is {peer.VirtualPlayer.Id}.");
                    foundAny = true;
                }
            }

            if (!foundAny)
                Helper.SendMessageToPeer(executor, $"Could not find any player whose name contains '{args}'.");

            return foundAny;
        }
    }
}