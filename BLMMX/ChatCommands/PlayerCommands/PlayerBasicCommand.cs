using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using BLMMX.Helpers;

namespace BLMMX.ChatCommands.PlayerCommands;

internal class PlayerBasicCommand : ChatCommand
{
    public override string CommandText => "s";

    public override string Description => "分配 ";

    public override bool CanExecute(NetworkCommunicator executor)
    {
        if (executor.IsAdmin) { return true; }
        return false;
    }

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        if (executor != null)
        {
            string playerId = executor.VirtualPlayer.Id.ToString();
            Mission.Current.ResetMission();
            Helper.SendMessageToPeer(executor, "Mission 重启");
        }
        return true;
    }
}
