using BLMMX.Entity;
using BLMMX.Helpers;
using HarmonyLib;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Patch;

[HarmonyPatch(typeof(MultiplayerRoundController), "EndRound")]
public class RoundPatch
{
    public static void Postfix(MultiplayerRoundController __instance)
    {

        MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
        if (missionScoreboardComponent == null)
        {
            return;
        }

        int attacker_score = missionScoreboardComponent.GetRoundScore(TaleWorlds.Core.BattleSideEnum.Attacker);
        int defender_score = missionScoreboardComponent.GetRoundScore(TaleWorlds.Core.BattleSideEnum.Defender);
        if (attacker_score > 2 || defender_score > 2 && __instance.RoundCount >= 3 && __instance.RoundCount <5)
        {
            MultiplayerTeamSelectComponent teamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();
            if (teamSelectComponent == null)
            {
                return;
            }

            Helper.SendMessageToAllPeers($"第一轮结束，比分为 {attacker_score}-{defender_score}");
            WillMatchData willMatchData = BLMMBehavior2.GetWillMatchData;

            foreach (NetworkCommunicator communicator in GameNetwork.NetworkPeers)
            {
                string playerId = Helper.GetPlayerId(communicator);
                if (willMatchData.firstTeamPlayerIds.Contains(playerId))
                {
                    teamSelectComponent.ChangeTeamServer(communicator, Mission.Current.DefenderTeam);
                }
                else if (willMatchData.secondTeamPlayerIds.Contains(playerId))
                {
                    teamSelectComponent.ChangeTeamServer(communicator, Mission.Current.AttackerTeam);
                }
            }

            for (int i = 0; i < missionScoreboardComponent.Sides.Length; i++)
            {
                MissionScoreboardComponent.MissionScoreboardSide? item = missionScoreboardComponent.Sides[i];
                item.SideScore = 0;
                missionScoreboardComponent.Sides[i] = item;
            }

            if (GameNetwork.IsServer)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new UpdateRoundScores(0, 0));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
            }

        }
        //MatchManager.SetMatchState(ESMatchState.FirstMatch);
        //Helper.PrintWarning("[MissionMultiplayerFlagDominationPatch]It is First");
    }
}
