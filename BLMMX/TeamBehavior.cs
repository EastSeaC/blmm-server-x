using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BLMMX
{
    internal class TeamBehavior : MultiplayerTeamSelectComponent
    {
        public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
        {
            base.OnAgentTeamChanged(prevTeam, newTeam, agent);


        }

        public void ExchangeTeams()
        {

        }
    }
}
