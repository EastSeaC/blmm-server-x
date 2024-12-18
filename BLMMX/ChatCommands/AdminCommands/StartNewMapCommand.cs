using BLMMX.Helpers;
using BLMMX.util;
using HarmonyLib;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ListedServer;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace BLMMX.ChatCommands.AdminCommands
{
    public class StartNewMapCommand : AdminChatCommand
    {
        public override string CommandText => "startmap";


        public override string Description => "启动新地图";

        public override bool Execute(NetworkCommunicator executor, string args)
        {
            if (executor == null) return false;

            try
            {
                if (args.Equals("1"))
                {
                    MBList<string> mapList = MultiplayerOptions.Instance.GetMapList();
                    foreach (string item in mapList)
                    {
                        Helper.PrintError("[es|map] " + item);
                    }
                    MultiplayerOptions.OptionType.Map.SetValue("mp_skirmish_map_003_skinc", MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);

                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    ReflectionHelper.SetField(serverSide, "_currentAutomatedBattleRemainingTime", 0f);
                    ReflectionHelper.SetField(serverSide, "_passedTimeSinceLastAutomatedBattleStateClientInform", 1f);
                    //ReflectionHelper.SetField(serverSide, "_remainedAutomatedBattleCount", 0);

                    //ReflectionHelper.InvokeMethod(serverSide, "SetAutomatedBattleState", new object[] { AutomatedBattleState.CountingForNextBattle });

                    MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = false;
                    MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;

                    //serverSide.SetIntermissionCultureVoting(false);
                    //serverSide.SetIntermissionCultureVoting(false);

                    serverSide.EndMission();

                    //serverSide.StartGameAndMission();
                }
                else if (args.Equals("2"))
                {
                    MultiplayerOptions.OptionType.Map.SetValue("mp_skirmish_map_003_skinc", MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
                    Helper.PrintError("[es|map] " + text);
                }
                else if (args.Equals("3"))
                {
                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    Helper.PrintError($"[es|AutomatedBattleState] {serverSide.AutomatedBattleState}");
                    // 当状态为 Idle 时 会停止
                    serverSide.StartMission();
                }
                else
                {
                    MultiplayerOptions.OptionType.Map.SetValue(args, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    MultiplayerOptionsExtensionsPatch.newmap_new = args;

                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    ReflectionHelper.SetField(serverSide, "_currentAutomatedBattleRemainingTime", 0f);
                    ReflectionHelper.SetField(serverSide, "_passedTimeSinceLastAutomatedBattleStateClientInform", 1f);

                    MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = false;
                    MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;
                    Helper.SendMessageToPeer(executor, "地图已保存");
                    serverSide.EndMission();
                }
                return true;
                //ReflectionHelper.InvokeMethod(ServerSideIntermissionManager.Instance, "")
            }
            catch (Exception ex)
            {
                Helper.SendMessageToPeer(executor, "执行错误" + ex.StackTrace);
                Helper.PrintError(ex.Message);
                Helper.PrintError(ex.StackTrace);
                return false;
            }

        }
    }

    public class ESMapManager
    {
        public static string MapName;
    }

    [HarmonyPatch(typeof(GameNetwork), nameof(GameNetwork.WriteMessage))]
    public class GameNetworkPatch
    {
        static bool Prefix(ref GameNetworkMessage message)
        {
            Type type = message.GetType();
            if (type == typeof(LoadMission))
            {
                LoadMission loadMission = message as LoadMission;
                Helper.PrintError("[es|map_patch|Prefix]" + loadMission.Map);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ServerSideIntermissionManager), "SelectMapAndFactions")]
    public class ServerSideIntermissionManagerPatch
    {
        static bool Prefix()
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
            Helper.PrintError("[es|SelectMapAndFactions]" + text);
            return true;
        }

        static void Postfix()
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
            Helper.PrintError("[es|SelectMapAndFactions|postfix]" + text);
        }
    }

    [HarmonyPatch(typeof(ServerSideIntermissionManager), "SelectRandomMap")]
    public class ServerSideIntermissionManagerPatch2
    {
        static void Postfix()
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
            Helper.PrintError("[es|SelectRandomMap|postfix]" + text);
        }
    }

    [HarmonyPatch(typeof(MultiplayerOptionsExtensions), nameof(MultiplayerOptionsExtensions.SetValue), new Type[] { typeof(MultiplayerOptions.OptionType), typeof(string), typeof(MultiplayerOptions.MultiplayerOptionsAccessMode) })]
    public class MultiplayerOptionsExtensionsPatch
    {
        public static string newmap_new = "";
        static bool Prefix(ref MultiplayerOptions.OptionType optionType, ref string value)
        {
            if (optionType == MultiplayerOptions.OptionType.Map)
            {
                Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch]" + value);
                if (newmap_new != null && !newmap_new.IsEmpty())
                {
                    //MultiplayerOptions.OptionType.Map.SetValue(newmap_new, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    //string cur_map_name = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    //Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|showMapName]" + cur_map_name);
                    return true;
                }
            }
            return true;
        }

        static async void Postfix()
        {
            if (newmap_new != null && !newmap_new.IsEmpty())
            {
                await Task.Delay(1000);
                MultiplayerOptions.OptionType.Map.SetValue(newmap_new, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);

            }

            string cur_map_name = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
            Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|protect_map_name]" + cur_map_name);
        }
    }
}
