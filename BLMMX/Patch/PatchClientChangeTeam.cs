using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        if (IsOpenBLMMMatch && playerTeams.TryGetValue(networkPeer, out Team team1))
        {
            if (team != team1)
            {
                return false;
            }
        }
        return true;
    }
}
