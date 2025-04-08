using BLMMX.Helpers;
using BLMMX.Util;
using NetworkMessages.FromServer;
using Newtonsoft.Json;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    internal class ChangeSocreCommand : AdminChatCommand
    {
        public override string CommandText => "cs";

        public override string Description => "修改第一轮比赛分数，使用模式 !cs 1， 将把轮数修改为4，并且两边都是2：2"
            + "\n使用！cs t2。将把轮数修改为9，并且两边都是2：2，需要等自动换边执行后再使用";


        public override bool Execute(NetworkCommunicator executor, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                return false;
            }
            if (args.StartsWith("t1")) // 比分改为  2：2 并且轮数改为4
            {
                MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
                if (missionScoreboardComponent == null)
                {
                    return false;
                }
                for (int i = 0; i < missionScoreboardComponent.Sides.Length; i++)
                {
                    MissionScoreboardComponent.MissionScoreboardSide? item = missionScoreboardComponent.Sides[i];
                    item.SideScore = 2;
                    missionScoreboardComponent.Sides[i] = item;
                }
                MultiplayerRoundController multiplayerRoundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
                if (multiplayerRoundController == null)
                {
                    return false;
                }
                multiplayerRoundController.RoundCount = 4;

                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new UpdateRoundScores(2, 2));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
                return true;
            }
            else if (args.StartsWith("t2"))
            {
                MissionScoreboardComponent missionScoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
                if (missionScoreboardComponent == null)
                {
                    return false;
                }
                for (int i = 0; i < missionScoreboardComponent.Sides.Length; i++)
                {
                    MissionScoreboardComponent.MissionScoreboardSide? item = missionScoreboardComponent.Sides[i];
                    item.SideScore = 2;
                    missionScoreboardComponent.Sides[i] = item;
                }
                MultiplayerRoundController multiplayerRoundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
                if (multiplayerRoundController == null)
                {
                    return false;
                }
                multiplayerRoundController.RoundCount = 9;

                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new UpdateRoundScores(2, 2));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
                return true;
            }
            else if (args.StartsWith("t3"))
            {
                MultiplayerTeamSelectComponent teamSelectComponent = Mission.Current.GetMissionBehavior<MultiplayerTeamSelectComponent>();
                if (teamSelectComponent == null)
                {
                    return false;
                }
                Team team = executor.GetComponent<MissionPeer>().Team;
                Team target_team = team == Mission.Current.AttackerTeam ? Mission.Current.DefenderTeam : Mission.Current.AttackerTeam;
                teamSelectComponent.ChangeTeamServer(executor, target_team);
            }
            else if (args.StartsWith("t4"))
            {
                MultiplayerRoundController multiplayerRoundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
                Helper.SendMessageToPeer(executor, $"RoundCount is {multiplayerRoundController.RoundCount}");
            }
            else if (args.StartsWith("t5"))
            {
                MultiplayerRoundController multiplayerRoundController = Mission.Current.GetMissionBehavior<MultiplayerRoundController>();
                string[] argsx = args.Split(' ');
                multiplayerRoundController.RoundCount = int.Parse(argsx[1]);
                Helper.SendMessageToPeer(executor, $"RoundCount is {multiplayerRoundController.RoundCount}");
            }
            else if (args.StartsWith("t6"))
            {
                try
                {
                    string message = JsonConvert.SerializeObject(BLMMBehavior2.DataContainer);
                    //Helper.SendMessageToPeer(executor, message);
                    Helper.PrintError(message);
                    File.WriteAllText(ModuleHelper.GetModuleFullPath("BLMMX") + "/DataContainer.json", message);
                }
                catch (Exception ex)
                {
                    return true;
                }

            }
            else if (args.StartsWith("t7"))
            {
                try
                {
                    string message = JsonConvert.SerializeObject(BLMMBehavior2.ConWillMatchData);
                    Helper.PrintError(message);
                    File.WriteAllText(ModuleHelper.GetModuleFullPath("BLMMX") + "/ConWillMatchData.json", message);
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
            else if (args.StartsWith("t8"))
            {
                string server_name = BLMMBehavior2.DataContainer.GetServername();
                var match_id = BLMMBehavior2.ConWillMatchData.matchId;
                NetUtil.Get($"cancel-match/{server_name}/{match_id}", new System.Text.Json.Nodes.JsonObject(), (res) =>
               {
                   Helper.PrintError(res);
               }, (ex) =>
               {
                   Helper.PrintError(ex.Message);
               });
            }
            return true;
        }
    }
}
