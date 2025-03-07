using BLMMX.Entity;
using BLMMX.Helpers;
using HarmonyLib;
using NetworkMessages.FromServer;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Patch;

public class RoundManager
{
    public static bool is_second_match = false;

}

[HarmonyPatch(typeof(MultiplayerRoundController), "BeginNewRound")]
public class RoundPatch2
{
    public static void Postfix()
    {
        MultiplayerRoundController roundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
        if (roundController == null)
        {
            return;
        }

        if (roundController.CurrentRoundState == MultiplayerRoundState.Preparation && RoundManager.is_second_match)
        {
            MultiplayerTeamSelectComponent teamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();
            if (teamSelectComponent == null)
            {
                return;
            }



            //if (willMatchData.firstTeamPlayerIds.Contains(playerId))
            //{
            //    Helper.Print($"[es|exchange_team] {playerId} {communicator.UserName} to DefenderTeam");
            //    teamSelectComponent.ChangeTeamServer(communicator, Mission.Current.DefenderTeam);
            //}
            //else if (willMatchData.secondTeamPlayerIds.Contains(playerId))
            //{
            //    Helper.Print($"[es|exchange_team] {playerId} {communicator.UserName} to AttackerTeam");
            //    teamSelectComponent.ChangeTeamServer(communicator, Mission.Current.AttackerTeam);
            //}
        }
    }
}


[HarmonyPatch(typeof(MultiplayerRoundController), "EndRound")]
public class RoundPatch
{
    public static async void Postfix()
    {
        MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
        if (missionScoreboardComponent == null)
        {
            return;
        }
        MultiplayerRoundController roundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
        if (roundController == null)
        {
            return;
        }
        BattleSideEnum battleSideEnum = roundController.RoundWinner;
        if (battleSideEnum == BattleSideEnum.Attacker)
        {
            BLMMBehavior2.DataContainer.AddAttackWinRoundNum();
        }
        else
        {
            BLMMBehavior2.DataContainer.AddDefendWinRoundNum();
        }
        int attacker_score = missionScoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
        int defender_score = missionScoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
        if ((attacker_score > 2 || defender_score > 2) && roundController.RoundCount <= 5)
        {
            RoundManager.is_second_match = true;

            MultiplayerTeamSelectComponent teamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();
            if (teamSelectComponent == null)
            {
                return;
            }

            Helper.SendMessageToAllPeers($"第一轮结束，比分为 {attacker_score}-{defender_score}");
            WillMatchData willMatchData = BLMMBehavior2.GetWillMatchData;
            // 根据比赛队伍名单，给予分数
            if (willMatchData != null)
            {
                BLMMBehavior2.DataContainer.SetAttackerSideScores(attacker_score);
                BLMMBehavior2.DataContainer.SetDefenderSideScores(defender_score);
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

            foreach (NetworkCommunicator communicator in GameNetwork.NetworkPeers)
            {
                string playerId = Helper.GetPlayerId(communicator);
                MissionPeer missionPeer = communicator.GetComponent<MissionPeer>();
                if (missionPeer != null)
                {
                    Team team = missionPeer.Team;

                    if (team != null && team != Mission.Current.SpectatorTeam)
                    {
                        Team targetteam = team == Mission.Current.AttackerTeam ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam;
                        teamSelectComponent.ChangeTeamServer(communicator, targetteam);
                    }
                }

            }


        }
        else if ((attacker_score > 2 || defender_score > 2) && RoundManager.is_second_match)
        {
            RoundManager.is_second_match = false;
            Helper.SendMessageToAllPeers($"第2轮结束，比分为 {attacker_score}-{defender_score}");
            WillMatchData willMatchData = BLMMBehavior2.GetWillMatchData;
            // 根据比赛队伍名单，给予分数
            if (willMatchData != null)
            {
                BLMMBehavior2.DataContainer.AddDefenderSideScores(attacker_score);
                BLMMBehavior2.DataContainer.AddAttackerSideScores(defender_score);
            }
            Helper.PrintWarning("[MissionMultiplayerFlagDominationPatch]" + JsonConvert.SerializeObject(BLMMBehavior2.DataContainer));
            ReflectionExtensions.GetMethodInfo(roundController, "PostMatchEnd").Invoke(roundController, new object[] { });
        }
        //MatchManager.SetMatchState(ESMatchState.FirstMatch);
        //Helper.PrintWarning("[MissionMultiplayerFlagDominationPatch]It is First");
    }
}
