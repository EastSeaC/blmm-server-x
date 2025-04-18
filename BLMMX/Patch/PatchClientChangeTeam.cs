﻿using BLMMX.Const;
using BLMMX.Entity;
using BLMMX.Helpers;
using BLMMX.util;
using HarmonyLib;
using NetworkMessages.FromClient;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;
using static TaleWorlds.MountAndBlade.Agent;

namespace BLMMX.Patch;

public class PatchClientChangeTeam
{
    //public static bool IsOpenBLMMMatch = false;
    //public static Dictionary<NetworkCommunicator, Team> playerTeams = new();101
    //public static bool PrefixChangeTeam(MultiplayerTeamSelectComponent __instance, NetworkCommunicator networkPeer, Team team)
    //{
    //    // 允许切旁观
    //    if (team == Mission.Current.SpectatorTeam) { return true; }


    //    if (__instance.GetPlayerCountForTeam(team) == 6)
    //    {
    //        Helper.SendMessageToPeer(networkPeer, "不可以超过 6 个人");
    //        return false;
    //    }
    //    return true;
    //}

    public static bool PrefixOnRoundEnd(MissionMultiplayerFlagDomination __instance, ref CaptureTheFlagCaptureResultEnum roundResult)
    {
        //if (roundResult == CaptureTheFlagCaptureResultEnum.AttackersWin)
        //{
        //    BLMMBehavior2.DataContainer.AddAttackWinRoundNum();
        //}
        //else
        //{
        //    BLMMBehavior2.DataContainer.AddDefendWinRoundNum();
        //}
        return true;
    }

    /// <summary>
    /// 比赛结束发送信息
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    public static bool PrefixOnPostMatchEnded(MultiplayerRoundController __instance)
    {
        ////////////////////////////////////////
        // 发送数据
        ////////////////////////////////////////
        Helper.Print("[es|PrefixOnPostMatchEnded] patch successfuly");
        //PlayerMatchDataContainer.TurnMatchToNorm();
        MultiplayerRoundController multiplayerRoundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
        MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
        if (missionScoreboardComponent != null)
        {
            if (BLMMBehavior2.ConWillMatchData != null)
            {
                BLMMBehavior2.DataContainer.SetAttackerSidePlayer(BLMMBehavior2.ConWillMatchData.firstTeamPlayerIds);
                BLMMBehavior2.DataContainer.SetDefebderSidePlayer(BLMMBehavior2.ConWillMatchData.secondTeamPlayerIds);
            }
            else
            {
                MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_attacker = missionScoreboardComponent.GetSideSafe(BattleSideEnum.Attacker);
                List<string> attack_player_ids = missionScoreboardSide_attacker.Players.Select(x => x.GetNetworkPeer().VirtualPlayer.Id.ToString()).ToList();
                MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_defender = missionScoreboardComponent.GetSideSafe(BattleSideEnum.Defender);
                List<string> defend_player_ids = missionScoreboardSide_defender.Players.Select(x => x.GetNetworkPeer().VirtualPlayer.Id.ToString()).ToList();
            }

            //BLMMBehavior2.DataContainer.SetAttackerSideScores(missionScoreboardSide_attacker.SideScore);
            //BLMMBehavior2.DataContainer.SetDefenderSideScores(missionScoreboardSide_defender.SideScore);
        }

        Dictionary<string, object> data = new()
        {
            ["MatchId"] = BLMMBehavior2.ConWillMatchData.matchId,
            ["IsMatchEnding"] = true,
            ["RoundCount"] = multiplayerRoundController.RoundCount + 1,
            ["MatchTime"] = DateTime.Now.ToString()
        };
        PlayerMatchDataContainer.SetTag(data);
        string result = JsonConvert.SerializeObject(BLMMBehavior2.DataContainer);
        Debug.Print(result);
        try
        {
            // 上传比赛数据
            HttpHelper.PostStringAsync(WebUrlManager.UploadMatchData, result);
            Debug.Print("[es|Sended DataToServer]");

        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message, color: Debug.DebugColor.Red);
            Debug.Print(ex.StackTrace, color: Debug.DebugColor.Red);
        }
        finally
        {
            //BLMMBehavior2.DataContainer.RefreshhAll();
            BLMMBehavior2.DataContainer = new();
        }

        // 换边
        if (MatchManager.MatchState == ESMatchState.FirstMatch)
        {
            Helper.Print("[MatchState|Switch2SecondMatch]");
            MatchManager.SetMatchState(ESMatchState.SecondMatch);

            MissionScoreboardComponent missionScoreboardComponent1 = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
            missionScoreboardComponent1.ChangeTeamScore(Mission.Current.Teams.Attacker, 0);
            missionScoreboardComponent1.ChangeTeamScore(Mission.Current.Teams.Defender, 0);
            MultiplayerTeamSelectComponent multiplayerTeamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();

            MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_attacker = missionScoreboardComponent1.Sides[0];
            List<NetworkCommunicator> networkCommunicator_attackers = new();
            if (missionScoreboardSide_attacker != null)
            {
                foreach (MissionPeer item in missionScoreboardSide_attacker.Players)
                {
                    if (item == null) { continue; }
                    NetworkCommunicator networkPeer = item.GetNetworkPeer();
                    networkCommunicator_attackers.Add(networkPeer);
                    multiplayerTeamSelectComponent.ChangeTeamServer(networkPeer, Mission.Current.SpectatorTeam);
                }
            }

            List<NetworkCommunicator> networkCommunicator_defenders = new();
            MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_defender = missionScoreboardComponent1.GetSideSafe(BattleSideEnum.Defender);
            foreach (MissionPeer? item in missionScoreboardSide_defender.Players)
            {
                NetworkCommunicator networkPeer = item.GetNetworkPeer();
                networkCommunicator_defenders.Add(networkPeer);
                multiplayerTeamSelectComponent.ChangeTeamServer(item.GetNetworkPeer(), Mission.Current.SpectatorTeam);
            }

            foreach (NetworkCommunicator item in networkCommunicator_defenders)
            {
                multiplayerTeamSelectComponent.ChangeTeamServer(item, Mission.Current.Teams.Attacker);
            }
            foreach (NetworkCommunicator item in networkCommunicator_attackers)
            {
                multiplayerTeamSelectComponent.ChangeTeamServer(item, Mission.Current.Teams.Defender);
            }

            __instance.RoundCount = 0;
            ReflectionHelper.SetProperty(__instance, "CurrentRoundState", MultiplayerRoundState.WaitingForPlayers);
            ReflectionHelper.InvokeMethod(__instance, "BeginNewRound", Array.Empty<object>());
            return false;
        }
        else if (MatchManager.MatchState == ESMatchState.SecondMatch)
        {
            // 踢出所有人，貌似有bug会闪退，但是踢出去就不会闪退
            //KickHelper.KickList(GameNetwork.NetworkPeers);
            MatchManager.SetMatchState(ESMatchState.None);
            Helper.PrintError("[MatchState|Switch2FirstMatch] kick off all players");
        }
        else if (MatchManager.MatchState == ESMatchState.MatchEnd)
        {
            MatchManager.SetMatchState(ESMatchState.None);
            Helper.PrintError("[MatchState|MatchEnd]");
            return true;
        }
        return true;
    }
}

public class MatchManager
{
    public static ESMatchState MatchState { get; set; }

    public static void SetMatchState(ESMatchState matchState)
    {
        MatchState = matchState;
    }
}

public enum ESMatchState
{
    None,
    FirstMatch,
    SecondMatch,
    MatchEnd,
}

public enum ESMatchType
{
    Match33,
    Match66,
    Match88,
    Test11,
    Test22,
    Test33,
}



[HarmonyPatch(typeof(MultiplayerTeamSelectComponent), "HandleClientEventTeamChange")]
public class MultiplayerTeamSelectComponentPatch
{
    private static MultiplayerTeamSelectComponent multiplayerTeamSelectComponent1;
    private static NetworkCommunicator networkCommunicator;
    private static GameNetworkMessage gameNetworkMessage;
    public static bool Prefix(MultiplayerTeamSelectComponent __instance, ref NetworkCommunicator peer, ref GameNetworkMessage baseMessage)
    {
        multiplayerTeamSelectComponent1 = __instance;
        networkCommunicator = peer;
        gameNetworkMessage = baseMessage;
        //return true;

        MultiplayerWarmupComponent multiplayerWarmupComponent = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();
        //if (multiplayerWarmupComponent != null && multiplayerWarmupComponent.IsInWarmup)
        //{
        //    Helper.Print("In warmup");
        //    return true;
        //}
        TeamChange teamChange = (TeamChange)baseMessage;
        Team team = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(teamChange.TeamIndex);
        // 允许切旁观
        if (team == Mission.Current.SpectatorTeam) { return true; }

        WillMatchData conWillMatchData = BLMMBehavior2.ConWillMatchData;
        if (conWillMatchData != null && !conWillMatchData.isCancel && !conWillMatchData.isFinished)
        {
            int max_num = conWillMatchData.GetTeamMaxNum();

            BasicCultureObject basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
            BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));

            Helper.PrintWarning($"[MultiplayerTeamSelectComponentPatch|MatchState] {MatchManager.MatchState}");

            switch (MatchManager.MatchState)
            {
                case ESMatchState.FirstMatch:
                    if (conWillMatchData.firstTeamPlayerIds.Contains(Helper.GetPlayerId(peer)))
                    {
                        if (team == Mission.Current.Teams[0])
                        {
                            Helper.PrintWarning($"[MultiplayerTeamSelectComponentPatch|FirstMatch] {peer.UserName} 选了1队");
                            return true;
                        }
                        else
                        {
                            Helper.SendMessageToPeer(peer, $"{peer.UserName},你的队伍是 2队");
                            return false;
                        }
                    }
                    else if (conWillMatchData.secondTeamPlayerIds.Contains(Helper.GetPlayerId(peer)))
                    {
                        if (team == Mission.Current.Teams[0])
                        {
                            Helper.SendMessageToPeer(peer, $"{peer.UserName},你的队伍是1队");
                            return false;
                        }
                        else
                        {
                            Helper.PrintWarning($"[MultiplayerTeamSelectComponentPatch|FirstMatch] {peer.UserName} 选了2队");
                            return true;
                        }
                    }
                    else
                    {
                        Helper.SendMessageToPeer(peer, "非比赛选手禁止选队伍");

                        return false;
                    }

                case ESMatchState.SecondMatch:
                    if (conWillMatchData.firstTeamPlayerIds.Contains(Helper.GetPlayerId(peer)) && team == Mission.Current.Teams[1])
                    {
                        Helper.PrintWarning($"[MultiplayerTeamSelectComponentPatch|SecondMatch] {peer.UserName} selected team2");
                        return true;
                    }
                    else if (conWillMatchData.secondTeamPlayerIds.Contains(Helper.GetPlayerId(peer)) && team == Mission.Current.Teams[0])
                    {
                        Helper.PrintWarning($"[MultiplayerTeamSelectComponentPatch|SecondMatch] {peer.UserName} selected team1");
                        return true;
                    }
                    else
                    {
                        Helper.SendMessageToPeer(peer, "非比赛选手禁止选队伍");
                        return false;
                    }
            }



            if (__instance.GetPlayerCountForTeam(team) == max_num)
            {
                if (peer != null)
                {
                    Helper.SendMessageToPeer(peer, $"不可以超过 {max_num} 个人");
                }
                return false;
            }
        }

        if (__instance.GetPlayerCountForTeam(team) == 8)
        {
            if (peer != null)
            {
                Helper.SendMessageToPeer(peer, "不可以超过 8 个人");
            }
            return false;
        }
        else
        {
            BLMMBehavior2.DataContainer.MarkPlayerNoSpecatator(Helper.GetPlayerId(peer));
        }

        return true;
    }

    public static void Postfix()
    {
        MultiplayerTeamSelectComponent __instance = multiplayerTeamSelectComponent1;
        NetworkCommunicator peer = networkCommunicator;
        TeamChange teamChange = (TeamChange)gameNetworkMessage;
        Team team = Mission.MissionNetworkHelper.GetTeamFromTeamIndex(teamChange.TeamIndex);

        // 允许切旁观
        if (team == Mission.Current.SpectatorTeam) { return; }


        WillMatchData conWillMatchData = BLMMBehavior2.ConWillMatchData;
        if (conWillMatchData != null && !conWillMatchData.isCancel && !conWillMatchData.isFinished)
        {
            string playerId = Helper.GetPlayerId(peer);
            if (!conWillMatchData.isplayerNeedMatch(playerId))
            {
                __instance.ChangeTeamServer(peer, Mission.Current.SpectatorTeam);
                Helper.SendMessageToPeer(peer, "非比赛选手禁止选队伍-[postfix]");
            }
        }
    }
}

[HarmonyPatch(typeof(MultiplayerTeamSelectComponent), "AutoAssignTeam")]
public class MultiplayerTeamSelectComponentPatchForAutoAssignTeam
{
    public static bool Prefix(MultiplayerTeamSelectComponent __instance, NetworkCommunicator peer)
    {
        __instance ??= Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();

        WillMatchData conWillMatchData = BLMMBehavior2.ConWillMatchData;
        if (conWillMatchData != null && !conWillMatchData.isCancel && !conWillMatchData.isFinished)
        {
            if (!conWillMatchData.isplayerNeedMatch(Helper.GetPlayerId(peer)))
            {
                __instance.ChangeTeamServer(peer, Mission.Current.SpectatorTeam);
                Helper.SendMessageToPeer(peer, "非比赛选手禁止选队伍");
                return false;
            }
            else if (MatchManager.MatchState == ESMatchState.FirstMatch)
            {
                if (conWillMatchData.isPlayerInFirstTeam(Helper.GetPlayerId(peer)))
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.AttackerTeam);
                }
                else if (conWillMatchData.isPlayerInSecondTeam(Helper.GetPlayerId(peer)))
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.DefenderTeam);
                }
                else
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.SpectatorTeam);
                }
                return false;
            }
            else if (MatchManager.MatchState == ESMatchState.SecondMatch)
            {
                if (conWillMatchData.isPlayerInFirstTeam(Helper.GetPlayerId(peer)))
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.DefenderTeam);
                }
                else if (conWillMatchData.isPlayerInSecondTeam(Helper.GetPlayerId(peer)))
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.AttackerTeam);
                }
                else
                {
                    __instance.ChangeTeamServer(peer, Mission.Current.SpectatorTeam);
                }
                return false;
            }
        }
        return true;
    }
}

