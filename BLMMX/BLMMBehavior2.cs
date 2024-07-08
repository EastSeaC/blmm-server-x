using BLMMX.Const;
using BLMMX.Helpers;
using BLMMX.MatchTable;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer;
using TaleWorlds.PlayerServices;
using Timer = TaleWorlds.Core.Timer;

namespace BLMMX;

internal class BLMMBehavior2 : MultiplayerTeamSelectComponent
{
    /// <summary>
    /// 检查时间为1s
    /// </summary>
    private static Timer timer;
    private static Timer timerIn2s;
    private static PlayerMatchDataContainer dataContainer = new();
    public BLMMBehavior2()
    {
        // 设置为0将会使得 只有当玩家进入后，MissionTimer才会启动计时
        timer = new(0f, 1f);
        timerIn2s = new(0f, 2f);
    }
    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();

        //GetMatchPlayerList();
    }

    /// <summary>
    /// 本模块为BLMM专用,BTL不需要
    /// </summary>
    public void GetMatchPlayerList()
    {
        try
        {
            string result = HttpHelper.DownloadStringTaskAsync(WebUrlManager.GetMatchList).Result;
            if (result != null)
            {
                dataContainer = JsonConvert.DeserializeObject<PlayerMatchDataContainer>(value: result);
                Debug.Print($"[OnBehaviorInitialize|同步网页端匹配列表成功]{result}");
            }
            else
            {
                Debug.Print($"[OnBehaviorInitialize|同步网页端匹配列表成功]{result}");
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            Debug.Print("[OnBehaviorInitialize|同步失败]");
            throw;
        }
    }

    /// 验证码 用于BLMM注册 <summary>
    /// 验证码 用于BLMM注册
    /// </summary>
    /// <param name="dt"></param>
    public async void GetPlayerVerifyCodeAsync(NetworkCommunicator networkCommunicator)
    {
        string PlayerId = networkCommunicator.VirtualPlayer.Id.ToString();
        try
        {
            string result = await HttpHelper.DownloadStringTaskAsync(WebUrlManager.GetVerifyCodeEx(PlayerId));

            if (result == null) return;
            PlayerRegInfo playerRegInfo = JsonConvert.DeserializeObject<PlayerRegInfo>(result);

            if (playerRegInfo == null) return;
            if (playerRegInfo.is_reg)
            {
                Helper.SendMessageToPeer(networkCommunicator, "欢迎来到BLMM服务器，你已经注册");
            }
            else
            {
                Helper.SendMessageToPeer(networkCommunicator, $"欢迎来到BLMM服务器，你的注册码是:{playerRegInfo.verify_code}");
            }
        }
        catch (Exception e)
        {
            Helper.PrintError(e.Message);
            Helper.PrintError(e.StackTrace);
            return;
        }
    }

    public async void SendPlayerName(NetworkCommunicator networkCommunicator)
    {
        string PlayerId = networkCommunicator.VirtualPlayer.Id.ToString();
        string name = networkCommunicator.UserName;

        try
        {
            string result = await HttpHelper.DownloadStringTaskAsync(WebUrlManager.SendPlayerName(PlayerId, name));
        }
        catch (Exception e)
        {
            Helper.PrintError(e.Message);
            Helper.PrintError(e.StackTrace);
            return;
        }
    }


    public override async void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);


        if (timerIn2s.Check(Mission.Current.CurrentTime))
        {
            Helper.PrintError($"[ES]CurPlayerNum:{GameNetwork.NetworkPeerCount}, Spectator:{Mission.GetMissionBehavior<MissionScoreboardComponent>().Spectators.Count}");
        }

        if (timer.Check(Mission.Current.CurrentTime))
        {
            //Helper.PrintError("[ES]ddd");

            /// 匹配列表
            //try
            //{
            //    string result = await HttpHelper.DownloadStringTaskAsync(WebUrlManager.GetMatchList);
            //    if (result != null)
            //    {
            //        Dictionary<string, string> d = new();
            //    }

            //}
            //catch (Exception ex) { }
            //string result = JsonConvert.SerializeObject(dataContainer, Formatting.Indented);
            // 刷新数据
            //Debug.Print(result);



        }
    }

    public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
    {
        Helper.SendMessageToAllPeers($"{networkPeer.UserName} leave server");
    }


    public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
    {
        string playerId = networkPeer.VirtualPlayer.Id.ToString();
        Helper.SendMessageToPeer(networkPeer, playerId);
        Helper.SendMessageToAllPeers($"Welcome {networkPeer.UserName} join server");

        dataContainer.AddPlayerWithName(networkPeer);
        //Debug.Print(JsonConvert.SerializeObject(dataContainer));
        GetPlayerVerifyCodeAsync(networkPeer);
        SendPlayerName(networkPeer);
        Debug.Print("[OnPlayerConnectedToServer|PlayerJoined]");

        KickWeirdBodyProperties(networkPeer);
        KickEmptyNames(networkPeer);
    }



    private bool KickWeirdBodyProperties(NetworkCommunicator networkPeer)
    {
        var vp = networkPeer.VirtualPlayer;
        var bodyProperties = vp.BodyProperties;
        ulong height = (bodyProperties.KeyPart8 >> 19) & 0x3F;
        if (height == 0)
        {
            return true;
        }
        if (height >= 15 && height <= 47) // Min/max height of the armory.
        {
            return false;
        }

        Helper.PrintError($"Kick player {vp.UserName} with a height of {height}");
        KickHelper.Kick(networkPeer, DisconnectType.KickedByHost, "bad_player_height");
        return true;
    }

    private bool KickEmptyNames(NetworkCommunicator networkPeer)
    {
        var vp = networkPeer.VirtualPlayer;
        if (!string.IsNullOrWhiteSpace(vp.UserName))
        {
            return false;
        }

        Helper.PrintError($"Kick player with an empty name \"{vp.UserName}\"");
        KickHelper.Kick(networkPeer, DisconnectType.KickedByHost, "empty_name");
        return true;
    }
    /// <summary>
    /// OnAgentCreated() 居然不执行
    /// </summary>
    /// <param name="agent"></param>
    public override void OnAgentControllerSetToPlayer(Agent agent)
    {
        base.OnAgentControllerSetToPlayer(agent);

        //if (agent.IsPlayerControlled)
        //{
        //    dataContainer.AddPlayerWithName(agent.MissionPeer);
        //    //string playerId = agent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
        //    //dataContainer.AddPlayerWithName(playerId, agent.MissionPeer.Name);
        //    Debug.Print("[OnAgentControllerSetToPlayer|Right]");
        //}
    }


    public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
    {
        base.OnAgentHit(affectedAgent, affectorAgent, affectorWeapon, blow, attackCollisionData);

        int InflictedDamage = blow.InflictedDamage;

        if (affectedAgent.IsPlayerControlled)
        {
            // 被攻击者是玩家
            if (affectorAgent.IsPlayerControlled)
            {
                //string affectedPlayerId = affectedAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();
                string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();

                if (affectedAgent.Team.TeamIndex == affectorAgent.Team.TeamIndex)
                {
                    dataContainer.AddTKValue(affectorPlayerId, InflictedDamage);
                }
                else
                {
                    dataContainer.AddPlayerAttackValue(affectorPlayerId, InflictedDamage);
                }
            }
            else if (affectorAgent.IsMount)
            {
                // 马攻击人
                Agent affectorRiderAgent = affectorAgent.RiderAgent;

                if (affectorRiderAgent != null)
                {
                    if (affectorRiderAgent.IsPlayerControlled)
                    {
                        string affectorPlayerId = affectorRiderAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                        dataContainer.AddPlayerAttackValue(affectorPlayerId, InflictedDamage);
                    }
                }
            }
        }
        else if (affectedAgent.IsMount)
        {
            Agent affectedRiderAgent = affectedAgent.RiderAgent;
            if (affectedRiderAgent != null)
            {
                if (affectorAgent.IsPlayerControlled)
                {
                    string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                    if (affectedRiderAgent.Team.TeamIndex == affectorAgent.Team.TeamIndex)
                    {
                        dataContainer.AddPlayerTKHorse(affectorPlayerId, InflictedDamage);
                    }
                    else
                    {
                        dataContainer.AddAttackHorse(affectorPlayerId, InflictedDamage);
                    }
                }
                // 马不可能攻击马
            }
            else // 无主马攻击。。。。 不计入
            {

            }
        }

        //Debug.Print($"[OnAgentHit|{blow.InflictedDamage}]");
        //Debug.Print($"[OnAgentHit|{JsonConvert.SerializeObject(dataContainer)}]");
    }


    protected override void OnEndMission()
    {
        base.OnEndMission();

        //foreach (NetworkCommunicator? peer in GameNetwork.NetworkPeers)
        //{
        //    GameNetwork.AddNetworkPeerToDisconnectAsServer(peer);
        //}
    }

    /// <summary>
    /// OnAgentControllerChanged 比 OnAgentControllerSetToPlayer 先执行，那么
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="oldController"></param>
    protected override void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
    {
        if (agent.IsPlayerControlled)
        {
            dataContainer.AddPlayerWithName(agent.MissionPeer);

            string PlayerId = agent.MissionPeer.GetPeer().Id.ToString();

            if (agent.MountAgent != null)
            {
                dataContainer.AddCalvary(PlayerId);
            }
            else if (agent.HasRangedWeapon())
            {
                dataContainer.AddArcher(PlayerId);
            }
            else
            {
                dataContainer.AddInfantryTimes(PlayerId);
            }
        }
    }

    public override void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {
        //string affectedPlayerId = affectedAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString(); 
        //string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Index.ToString();
        Helper.Print("[OnEarlyAgentRemoved|Tim]");
        if (affectedAgent.IsHuman && affectedAgent.IsPlayerControlled)
        {
            string affectedPlayerId = affectedAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();

            if (affectorAgent.IsHuman && affectorAgent.IsPlayerControlled)
            {
                string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                if (affectorAgent.Team.IsEnemyOf(affectedAgent.Team))
                {
                    dataContainer.AddKillNumber2(affectorPlayerId);
                }
                else
                {
                    dataContainer.AddPlayerTK(affectorPlayerId);
                }
            }
            else if (affectorAgent.IsMount)
            {
                Agent affectorRiderAgent = affectorAgent.RiderAgent;
                if (affectorRiderAgent != null)
                {
                    if (affectorRiderAgent.IsPlayerControlled)
                    {
                        // 玩家骑马撞死敌人
                        string affectorPlayerId = affectorAgent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                        dataContainer.AddKillNumber(affectorPlayerId);
                    }
                }
            }
        }
        else if (affectedAgent.IsMount)
        {

        }
    }
    public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
    {

        //MissionScoreboardComponent scoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();

        //foreach (NetworkCommunicator item in GameNetwork.NetworkPeers)
        //{
        //    if (item.ControlledAgent != null)
        //    {
        //        MissionPeer missionPeer = item.ControlledAgent.MissionPeer;

        //        if (missionPeer != null)
        //        {
        //            dataContainer.SetKillNumber(item.VirtualPlayer.Id.ToString(), missionPeer.KillCount);
        //        }
        //    }
        //}
    }


    public override void OnEndMissionInternal()
    {
        base.OnEndMissionInternal();

        //Mission.Current.ResetMission();
        //GameNetwork.NetworkPeers.Clear();
    }

    public override void OnAgentDeleted(Agent affectedAgent)
    {
        base.OnAgentDeleted(affectedAgent);

        if (affectedAgent.IsPlayerControlled)
        {

        }

        //MissionScoreboardComponent scoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();
        //if (scoreboardComponent != null)
        //{

        //}
        //var k = scoreboardComponent.GetSideSafe(BattleSideEnum.Defender);

        //if (Mission.Agents.Count == 0)
        //{

        //}
    }

    
    public override void OnClearScene()
    {
        base.OnClearScene();

        //if (k == null)
        //{
        //    Debug.Print("OnClearScene|IS NULL");
        //}
        //else
        //{
        //    Debug.Print($"OnClearScene|{k.IsInWarmup}");
        //}
        try
        {
            MultiplayerRoundController multiplayerRoundController = Mission.GetMissionBehavior<MultiplayerRoundController>();
            if (multiplayerRoundController == null)
            {
                Helper.PrintError("[OnClearScene|multiplayerRoundController is null]");
                return;
            }

            MissionScoreboardComponent scoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();


            foreach (MissionPeer missionPeer in scoreboardComponent.GetSideSafe(BattleSideEnum.Attacker).Players)
            {
                string PlayerId = missionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                dataContainer.SetKillNumber(PlayerId, missionPeer.KillCount);
                dataContainer.SetDeathNumber(PlayerId, missionPeer.DeathCount);
                dataContainer.SetAssistNumber(PlayerId, missionPeer.AssistCount);
                // 添加进攻队伍
                PlayerMatchDataContainer.AddAttackPlayer(PlayerId);
                dataContainer.SetRoundScore(PlayerId, scoreboardComponent.GetRoundScore(BattleSideEnum.Attacker));
                if (multiplayerRoundController.RoundWinner == BattleSideEnum.Attacker)
                {
                    dataContainer.AddAttackWinRoundNum();
                    dataContainer.AddWinRound(PlayerId);
                    dataContainer.AddLoseRound(PlayerId);
                }
                else
                {
                    dataContainer.AddDefendWinRoundNum();
                    dataContainer.AddLoseRound(PlayerId);
                }


            }
            foreach (MissionPeer missionPeer in scoreboardComponent.GetSideSafe(BattleSideEnum.Defender).Players)
            {
                string PlayerId = missionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                dataContainer.SetKillNumber(PlayerId, missionPeer.KillCount);
                dataContainer.SetDeathNumber(PlayerId, missionPeer.DeathCount);
                dataContainer.SetAssistNumber(PlayerId, missionPeer.AssistCount);
                // 添加防御队伍
                PlayerMatchDataContainer.AddDefendPlayer(PlayerId);
                dataContainer.SetRoundScore(PlayerId, scoreboardComponent.GetRoundScore(BattleSideEnum.Defender));
                if (multiplayerRoundController.RoundWinner == BattleSideEnum.Defender)
                {
                    dataContainer.AddAttackWinRoundNum();
                    dataContainer.AddWinRound(PlayerId);
                }
                else
                {
                    dataContainer.AddDefendWinRoundNum();
                    dataContainer.AddLoseRound(PlayerId);
                }
            }

            Dictionary<string, object> data = new()
            {
                ["IsMatchEnding"] = multiplayerRoundController.IsMatchEnding,
                ["RoundCount"] = multiplayerRoundController.RoundCount,
            };
            PlayerMatchDataContainer.SetTag(data);
        }
        catch (Exception e)
        {
            Helper.PrintError(e.Message);
            Helper.PrintError(e.StackTrace);
            return;
        }

        ////////////////////////////////////////
        // 发送数据
        ////////////////////////////////////////
        MultiplayerWarmupComponent k = Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
        if (k == null)
        {
            PlayerMatchDataContainer.TurnMatchToNorm();
            string result = JsonConvert.SerializeObject(dataContainer);
            Debug.Print(result);
            try
            {
                HttpHelper.PostStringAsync(WebUrlManager.UploadMatchData, result);
                Debug.Print("[OnClearScene|Sended DataToServer]");

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message, color: Debug.DebugColor.Red);
                Debug.Print(ex.StackTrace, color: Debug.DebugColor.Red);
                return;
            }

        }
        else
        {
            PlayerMatchDataContainer.TurnMatchToTest();
        }
        // 刷新数据
        dataContainer.RefreshhAll();
    }
}
