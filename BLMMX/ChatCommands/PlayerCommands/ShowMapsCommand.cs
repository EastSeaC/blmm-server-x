using BLMMX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ListedServer;

namespace BLMMX.ChatCommands.PlayerCommands;

public class ShowMapsCommand : ChatCommand
{
    public override string CommandText => "show_maps";

    public override string Description => "显示地图列表";

    public override bool CanExecute(NetworkCommunicator executor)
    {
        return true;
    }

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        int count = 0;
        string xz = "地图包:\n";
        foreach (string? i in ServerSideIntermissionManager.Instance.AutomatedMapPool)
        {
            xz += $"索引{count}:{i}\n";
            count++;
        }
        Helper.SendMessageToPeer(executor, xz);

        ServerSideIntermissionManager.Instance.SetServerName("BLMM");

        
        return true;
    }
}
