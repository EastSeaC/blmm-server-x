using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Patch;

public class PatchWorker
{
    public static void Apply(Harmony harmony)
    {
        //Harmony harmony = new("com.es.patch");
        //harmony.PatchAll();

        //AddPrefix(harmony, typeof(MultiplayerTeamSelectComponent), "ChangeTeamServer", BindingFlags.Public | BindingFlags.Instance,
        //    typeof(PatchClientChangeTeam), nameof(PatchClientChangeTeam.PrefixChangeTeam));
        AddPrefix(harmony, typeof(MultiplayerRoundController), "PostMatchEnd", BindingFlags.NonPublic | BindingFlags.Instance,
            typeof(PatchClientChangeTeam), nameof(PatchClientChangeTeam.PrefixOnPostMatchEnded));

        AddPrefix(harmony, typeof(MultiplayerTeamSelectComponent), "ChangeTeamServer", BindingFlags.Public | BindingFlags.Instance, 
            typeof(MultiplayerTeamSelectComponentPatch), nameof(MultiplayerTeamSelectComponentPatch.Prefix));
        //AddPrefix(harmony, typeof(MissionMultiplayerFlagDomination), "OnWarmupEnding", BindingFlags.NonPublic | BindingFlags.Instance,
        //    typeof(MissionMultiplayerFlagDominationPatch), nameof(MissionMultiplayerFlagDominationPatch.Postfix));
        //AddPrefix(harmony, typeof(MultiplayerRoundController), "EndRound", BindingFlags.NonPublic | BindingFlags.Instance,
        //    typeof(PatchClientChangeTeam), nameof(PatchClientChangeTeam.PrefixOnRoundEnd));
    }

    private static void AddPrefix(Harmony harmony, Type classToPatch, string functionToPatchName, BindingFlags flags, Type patchClass, string functionPatchName)
    {
        var functionToPatch = classToPatch.GetMethod(functionToPatchName, flags);
        var newHarmonyPatch = patchClass.GetMethod(functionPatchName);
        harmony.Patch(functionToPatch, prefix: new HarmonyMethod(newHarmonyPatch));
    }
}
