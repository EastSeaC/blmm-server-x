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
                        Helper.SendMessageToPeer(executor, "[es|map] " + item);
                    }
                    MultiplayerOptions.OptionType.Map.SetValue("mp_skirmish_map_003_skinc", MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);

                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    ReflectionHelper.SetField(serverSide, "_currentAutomatedBattleRemainingTime", 0f);
                    ReflectionHelper.SetField(serverSide, "_passedTimeSinceLastAutomatedBattleStateClientInform", 1f);
                    //ReflectionHelper.SetField(serverSide, "_remainedAutomatedBattleCount", 0);

                    //ReflectionHelper.InvokeMethod(serverSide, "SetAutomatedBattleState", new object[] { AutomatedBattleState.CountingForNextBattle });

                    MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = true;
                    MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;

                    //serverSide.SetIntermissionCultureVoting(false);
                    //serverSide.SetIntermissionCultureVoting(false);
                    try
                    {
                        bool IsAssignale = (bool)ReflectionHelper.InvokeMethod(serverSide, "IsNewTaskAssignable", Array.Empty<object>());
                        ReflectionHelper.SetField(serverSide, "_currentTask", null);
                        Mission.Current.EndMission();
                        serverSide.EndMission();
                    }
                    catch (Exception ex)
                    {
                        Helper.PrintError("es|www  ");
                    }


                    //serverSide.StartGameAndMission();
                }
                else if (args.Equals("22"))
                {
                    KickHelper.KickList(GameNetwork.NetworkPeers);
                    MultiplayerOptions.OptionType.Map.SetValue("mp_bnl_lighthouse", MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    ReflectionHelper.SetField(serverSide, "_currentAutomatedBattleRemainingTime", 0f);
                    ReflectionHelper.SetField(serverSide, "_passedTimeSinceLastAutomatedBattleStateClientInform", 1f);
                    ReflectionHelper.SetField(serverSide, "_remainedAutomatedBattleCount", 0);

                    MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = false;
                    MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;

                    try
                    {
                        //bool IsAssignale = (bool)ReflectionHelper.InvokeMethod(serverSide, "IsNewTaskAssignable", Array.Empty<object>());
                        ReflectionHelper.SetField(serverSide, "_currentTask", null);
                        Mission.Current.EndMission();
                        serverSide.EndMission();
                    }
                    catch (Exception ex)
                    {
                        Helper.PrintError("es|www  ");
                    }
                }
                else if (args.Equals("666"))
                {
                    Mission.Current.EndMission();
                }
                else if (args.Equals("2"))
                {
                    MultiplayerOptions.OptionType.Map.SetValue("mp_bnl_lighthouse", MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
                    Helper.PrintError("[es|map] " + text);
                    Helper.SendMessageToPeer(executor, "当前地图为" + text);
                }
                else if (args.Equals("3"))
                {
                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    if (args.Equals("31"))
                    {
                        MultiplayerOptions.OptionType.CultureTeam1.SetValue("khuzait");
                        MultiplayerOptions.OptionType.CultureTeam1.SetValue("battania");
                    }
                    else if (args.Equals("32"))
                    {
                        MultiplayerOptions.OptionType.CultureTeam1.SetValue("sturgia");
                        MultiplayerOptions.OptionType.CultureTeam1.SetValue("battania");
                    }
                    Helper.PrintError($"[es|AutomatedBattleState] {serverSide.AutomatedBattleState}");
                    // 当状态为 Idle 时 会停止
                    serverSide.StartMission();
                }
                else
                {
                    MultiplayerOptionsExtensionsPatch.newmap_new = args;
                    MultiplayerOptions.OptionType.Map.SetValue(args, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);

                    ServerSideIntermissionManager serverSide = ServerSideIntermissionManager.Instance;
                    ReflectionHelper.SetField(serverSide, "_currentAutomatedBattleRemainingTime", 0f);
                    ReflectionHelper.SetField(serverSide, "_passedTimeSinceLastAutomatedBattleStateClientInform", 1f);
                    ReflectionHelper.SetField(serverSide, "_remainedAutomatedBattleCount", 0);

                    MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = false;
                    MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;
                    Helper.SendMessageToPeer(executor, "地图已保存");
                    KickHelper.KickList(GameNetwork.NetworkPeers);
                    ReflectionHelper.SetField(serverSide, "_currentTask", null);
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

    //[HarmonyPatch(typeof(GameNetwork), nameof(GameNetwork.WriteMessage))]
    public class GameNetworkPatch
    {
        static bool Prefix(ref GameNetworkMessage message)
        {
            Type type = message.GetType();
            if (type == typeof(LoadMission))
            {
                LoadMission loadMission = (LoadMission)message;

                Helper.PrintError("[es|map_patch|Prefix]" + loadMission.Map);
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(ServerSideIntermissionManager), "SelectMapAndFactions")]
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

    //[HarmonyPatch(typeof(ServerSideIntermissionManager), "EndMission")]
    //public class ServerSideIntermissionManagerPatch2
    //{
    //    static async void Postfix()
    //    {
    //        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text);
    //        Helper.PrintError("[es|SelectRandomMap|postfix]" + text);
    //        await Task.Delay(18 * 1000);

    //        ServerSideIntermissionManager instance = ServerSideIntermissionManager.Instance;
    //        instance.StartMission();

    //        await Task.Delay(5 * 1000);
    //        if (instance.AutomatedBattleState == AutomatedBattleState.Idle)
    //        {
    //            ReflectionHelper.SetField(instance, "_currentTask", null);
    //            instance.StartMission();
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(MultiplayerOptionsExtensions), nameof(MultiplayerOptionsExtensions.SetValue), new Type[] { typeof(MultiplayerOptions.OptionType), typeof(string), typeof(MultiplayerOptions.MultiplayerOptionsAccessMode) })]
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
                    if (newmap_new.Equals(value))
                    {
                        return true;
                    }
                    AutomatedBattleState automatedBattleState = ServerSideIntermissionManager.Instance.AutomatedBattleState;
                    if (automatedBattleState == AutomatedBattleState.AtBattle)
                    {
                        return true;
                    }

                    //MultiplayerOptions.OptionType.Map.SetValue(newmap_new, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    //string cur_map_name = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                    //Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|showMapName]" + cur_map_name);
                    return true;
                }
            }
            else if (optionType == MultiplayerOptions.OptionType.CultureTeam2)
            {
                Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|CultureTeam2]" + value);
                if (value != "vlandia")
                {
                    MultiplayerOptions.OptionType.CultureTeam2.SetValue("vlandia");
                }
                return false;
            }
            else if (optionType == MultiplayerOptions.OptionType.CultureTeam1)
            {
                Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|CultureTeam1]" + value);
            }
            return true;
        }

        static async void Postfix()
        {
            string cur_map_name = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
            if (newmap_new != null && !newmap_new.IsEmpty())
            {
                if (cur_map_name.Equals(newmap_new))
                {
                    return;
                }
                MultiplayerOptions.OptionType.Map.SetValue(newmap_new, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
                ServerSideIntermissionManager.Instance.StartMission();
                Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|triggerProtect]");
            }


            Helper.PrintError("[es|MultiplayerOptionsExtensionsPatch|protect_map_name]" + cur_map_name);

        }
    }
}
