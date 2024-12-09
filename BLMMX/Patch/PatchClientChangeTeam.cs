using BLMMX.Const;
using BLMMX.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer;

namespace BLMMX.Patch;

public class PatchClientChangeTeam
{
    public static bool IsOpenBLMMMatch = false;

    public static Dictionary<NetworkCommunicator, Team> playerTeams = new();
    public static bool PrefixChangeTeam(MultiplayerTeamSelectComponent __instance, NetworkCommunicator networkPeer, Team team)
    {
        // 允许切旁观
        if (team == Mission.Current.SpectatorTeam) { return true; }


        if (IsOpenBLMMMatch && playerTeams.TryGetValue(networkPeer, out Team team1))
        {
            if (team != team1)
            {
                return false;
            }
        }
        return true;
    }

    public static bool PrefixOnRoundEnd(MissionMultiplayerFlagDomination __instance, ref CaptureTheFlagCaptureResultEnum roundResult)
    {
        if (roundResult == CaptureTheFlagCaptureResultEnum.AttackersWin)
        {
            BLMMBehavior2.DataContainer.AddAttackWinRoundNum();
        }
        else
        {
            BLMMBehavior2.DataContainer.AddDefendWinRoundNum();
        }
        return true;
    }

    public static bool PrefixOnPostRoundEnded(MultiplayerRoundController __instance)
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

            BLMMBehavior2.DataContainer.SetDefenderSideScores(missionScoreboardComponent.GetSideSafe(TaleWorlds.Core.BattleSideEnum.Defender).SideScore);
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
