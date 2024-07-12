using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.DotNet;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ListedServer;
using static System.Net.Mime.MediaTypeNames;

namespace BLMMX.ChatCommands.AdminCommands;

public class SetMapCommand : AdminChatCommand
{
    public override string CommandText => "set_map";

    public override string Description => "设置地图";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        return false;
        if (string.IsNullOrWhiteSpace(args)) return false;

        if (int.TryParse(args, out var id))
        {
            ServerSideIntermissionManager manager = ServerSideIntermissionManager.Instance;
            if (id < 0 || id >= manager.AutomatedBattleCount) return false;
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out string gameType);
            string mapName = manager.AutomatedMapPool[id];
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new LoadMission(gameType, mapName, 1));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients, null);
            if (!Module.CurrentModule.StartMultiplayerGame(gameType, mapName))
            {
                Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "ServerSideIntermissionManager.cs", "StartMissionAux", 726);
            }

            return true;
        }
        else if (ServerSideIntermissionManager.Instance.AutomatedMapPool.Contains(args))
        {
            string mapName = args;
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out string gameType);
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new LoadMission(gameType, mapName, 1));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients, null);
            if (!Module.CurrentModule.StartMultiplayerGame(gameType, mapName))
            {
                Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "ServerSideIntermissionManager.cs", "StartMissionAux", 716);
            }
        }

        return false;
    }
}
