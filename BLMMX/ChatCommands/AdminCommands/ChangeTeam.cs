using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    public class ChangeTeam : AdminChatCommand
    {
        public override string CommandText => "change_team";

        public override string Description => "c";

        public override bool Execute(NetworkCommunicator executor, string args)
        {
            MissionScoreboardComponent scoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
            MultiplayerTeamSelectComponent teamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();
            if (teamSelectComponent == null) return false;

            if (scoreboardComponent == null) return false;

            MissionScoreboardComponent.MissionScoreboardSide side_a = scoreboardComponent.GetSideSafe(BattleSideEnum.Attacker);
            MissionScoreboardComponent.MissionScoreboardSide side_b = scoreboardComponent.GetSideSafe(BattleSideEnum.Defender);
            IEnumerable<MissionPeer> attackerPlayers = side_a.Players;
            IEnumerable<MissionPeer> defenderPlayers = side_b.Players;
            foreach (MissionPeer? item in attackerPlayers)
            {
                teamSelectComponent.ChangeTeamServer(item.GetNetworkPeer(), Mission.Current.DefenderTeam);
            }

            foreach (MissionPeer? item in defenderPlayers)
            {
                teamSelectComponent.ChangeTeamServer(item.GetNetworkPeer(), Mission.Current.AttackerTeam);
            }

            return true;
        }
    }
}
