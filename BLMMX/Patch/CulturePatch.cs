using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ListedServer;
using static System.Net.Mime.MediaTypeNames;

namespace BLMMX.Patch
{
    //[HarmonyPatch(typeof(MultiplayerOptions.MultiplayerOption), nameof(MultiplayerOptions.MultiplayerOption.UpdateValue))]
    internal class CulturePatch
    {
        private static bool Prefix(MultiplayerOptions.MultiplayerOption __instance)
        {
            if (__instance.OptionType == MultiplayerOptions.OptionType.CultureTeam1)
            {
                string culcute = string.Empty;
                if (BLMMBehavior2.ConWillMatchData != null)
                {
                    culcute = BLMMBehavior2.ConWillMatchData.firstTeamCultrue;
                }
                Traverse.Create(__instance).Field("_stringValue").SetValue(culcute);
                return false;
            }
            else if (__instance.OptionType == MultiplayerOptions.OptionType.CultureTeam2)
            {
                string culcute = string.Empty;
                if (BLMMBehavior2.ConWillMatchData != null)
                {
                    culcute = BLMMBehavior2.ConWillMatchData.secondTeamCultrue;
                }
                Traverse.Create(__instance).Field("_stringValue").SetValue(culcute);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ServerSideIntermissionManager), "StartMissionAux")]
    public class FactionPatch
    {
        private static bool Prefix(ServerSideIntermissionManager __instance)
        {
            
            
            return true;
        }
    }
}
