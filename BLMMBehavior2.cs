using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BLMMX
{
    internal class BLMMBehavior2 : MissionNetwork
    {
        private Dictionary<string, PlayerMatchData> playerMatchData = new();
        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

        }
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

        }

        public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
        {
            base.OnPlayerConnectedToServer(networkPeer);

            string playerId = networkPeer.VirtualPlayer.Id.ToString();
            if (!playerMatchData.ContainsKey(playerId))
            {
                playerMatchData.Add(playerId, new(playerId));
            }

            Debug.Print("[OnPlayerConnectedToServer|PlayerJoined]");
        }

        

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
        {
            base.OnAgentHit(affectedAgent, affectorAgent, affectorWeapon, blow, attackCollisionData);

            string affectedPlayerId = affectedAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();
            string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();

            if (affectedAgent.IsHuman)
            {
                if (!affectedAgent.IsPlayerControlled)
                {
                    return;
                }
                if (affectedAgent.Team.TeamIndex == affectorAgent.Team.TeamIndex)
                {
                    if (playerMatchData.ContainsKey(affectedPlayerId))
                    {
                        playerMatchData[affectorPlayerId].AddTK(blow.InflictedDamage);
                    }
                }
                else
                {
                    if (playerMatchData.ContainsKey(affectorPlayerId))
                    {
                        playerMatchData[affectorPlayerId].AddAttack(blow.InflictedDamage);
                    }
                }
            }
            else
            {
                // 是马
                if (affectedAgent.RiderAgent != null)
                {
                    if (affectedAgent.RiderAgent.Team.TeamIndex == affectorAgent.Team.TeamIndex)
                    {
                        playerMatchData[affectorPlayerId].AddTKHorse(blow.InflictedDamage);
                    }
                    else
                    {
                        playerMatchData[affectorPlayerId].AddAttackHorse(blow.InflictedDamage);
                    }
                }
            }

            Debug.Print("[OnAgentHit|baga]");
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            string affectedPlayerId = affectedAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();
            string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();
            

            if (affectedAgent.IsHuman)
            {

            }
            else
            {

            }
        }
        public override void OnClearScene()
        {
            base.OnClearScene();

            Debug.Print("[YES]");
        }
    }
}
