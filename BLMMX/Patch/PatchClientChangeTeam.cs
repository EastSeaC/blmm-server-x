using BLMMX.Const;
using BLMMX.Helpers;
using HarmonyLib;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Patch;

public class PatchClientChangeTeam
{
    public static bool IsOpenBLMMMatch = false;

    public static Dictionary<NetworkCommunicator, Team> playerTeams = new();
    public static bool PrefixChangeTeam(MultiplayerTeamSelectComponent __instance, NetworkCommunicator networkPeer, Team team)
    {
        // 允许切旁观
        if (team == Mission.Current.SpectatorTeam) { return true; }


        if (__instance.GetPlayerCountForTeam(team) == 6)
        {
            Helper.SendMessageToPeer(networkPeer, "不可以超过 6 个人");
            return false;
        }
        return true;
    }

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
        Helper.Print("[es|send data to web]");
        //PlayerMatchDataContainer.TurnMatchToNorm();
        MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
        if (missionScoreboardComponent != null)
        {
            MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_attacker = missionScoreboardComponent.GetSideSafe(TaleWorlds.Core.BattleSideEnum.Attacker);
            List<string> attack_player_ids = missionScoreboardSide_attacker.Players.Select(x => x.GetNetworkPeer().VirtualPlayer.Id.ToString()).ToList();
            BLMMBehavior2.DataContainer.SetAttackerSidePlayer(attack_player_ids);

            BLMMBehavior2.DataContainer.SetAttackerSideScores(missionScoreboardSide_attacker.SideScore);

            MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_defender = missionScoreboardComponent.GetSideSafe(TaleWorlds.Core.BattleSideEnum.Defender);
            List<string> defend_player_ids = missionScoreboardSide_defender.Players.Select(x => x.GetNetworkPeer().VirtualPlayer.Id.ToString()).ToList();
            BLMMBehavior2.DataContainer.SetDefenderSideScores(missionScoreboardSide_defender.SideScore);
        }

        string result = JsonConvert.SerializeObject(BLMMBehavior2.DataContainer);
        Debug.Print(result);
        try
        {
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
        return true;
    }
}

[HarmonyPatch(typeof(MultiplayerTeamSelectComponent), "ChangeTeamServer")]
public class MultiplayerTeamSelectComponentPatch
{
    static bool Prefix(MultiplayerTeamSelectComponent __instance, ref NetworkCommunicator networkPeer, ref Team team)
    {
        // 允许切旁观
        if (team == Mission.Current.SpectatorTeam) { return true; }


        if (__instance.GetPlayerCountForTeam(team) == 6)
        {
            if (networkPeer != null)
            {
                Helper.SendMessageToPeer(networkPeer, "不可以超过 6 个人");
            }
            return false;
        }

        return true;
    }
}