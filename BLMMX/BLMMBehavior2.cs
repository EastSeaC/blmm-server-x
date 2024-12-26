using BLMMX.Const;
using BLMMX.Entity;
using BLMMX.Helpers;
using BLMMX.MatchTable;
using BLMMX.Util;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using Timer = TaleWorlds.Core.Timer;

namespace BLMMX;

internal class BLMMBehavior2 : MultiplayerTeamSelectComponent
{
    /// <summary>
    /// 检查时间为1s
    /// </summary>
    private Timer timerIn1s;
    private Timer timerIn3s;
    private Timer timerIn5s;
    private static PlayerMatchDataContainer dataContainer;
    private static WillMatchData willMatchData;

    private static int MaxAfterPlayerArrivadedWatingCount = 180;
    private static int AfterPlayerArrivadedWatingCount;

    public static async void MarkCurrentMatchCancel(string reason = "auto")
    {
        if (willMatchData == null)
        {
            return;
        }

        willMatchData.isCancel = true;
        willMatchData.cancelReason = reason;


        await NetUtil.GetAsync("/cancel-match", (res) =>
        {
            if (res.IsEmpty())
            {
                Helper.SendMessageToAllPeers("比赛取消异常，请联系管理员");
                return;
            }
            else
            {
                EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(res);
                if (eWebResponse == null)
                {
                    return;
                }

                if (eWebResponse.code == 0)
                {
                    Helper.SendMessageToAllPeers("对局取消，踢出所有人");
                }
            }
        }, (e) =>
        {

        });
        await Task.Delay(3000);
        KickHelper.KickList(GameNetwork.NetworkPeers);
        //HttpHelper.DownloadStringTaskAsync()
    }

    public static PlayerMatchDataContainer DataContainer
    {
        get => dataContainer; set { dataContainer = value; }
    }

    public static WillMatchData ConWillMatchData
    {
        get => willMatchData;
        set => willMatchData = value;
    }

    private static bool IsRegEvent = false;
    public BLMMBehavior2()
    {
        // 设置为0将会使得 只有当玩家进入后，MissionTimer才会启动计时
        timerIn1s = new(0f, 1f);
        timerIn3s = new(0f, 3f);
        timerIn5s = new(0f, 5f);

        dataContainer ??= new();
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out string text7);
        dataContainer.SetServername(text7);

        willMatchData = WillMatchData.GetFake();

        AfterPlayerArrivadedWatingCount = 0;
    }
    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();

        //GetMatchPlayerList();
        dataContainer ??= new();

        AfterPlayerArrivadedWatingCount = 0;

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
                Debug.Print($"[OnBehaviorInitialize|同步网页端匹配列表失败]{result}");
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
            PlayerRegInfo? playerRegInfo = JsonConvert.DeserializeObject<PlayerRegInfo>(result);

            if (playerRegInfo == null) return;
            Helper.SendMessageToPeer(networkCommunicator, "QQ群：983357051");
            if (playerRegInfo.is_reg)
            {
                Helper.SendMessageToPeer(networkCommunicator, "欢迎来到BLMM服务器，你已经注册");
            }
            else
            {
                Helper.SendMessageToPeer(networkCommunicator, $"欢迎来到BLMM服务器，你的playerId（steamId)是:{playerRegInfo.player_id},你的注册码是:{playerRegInfo.verify_code}");
                Helper.SendMessageToPeer(networkCommunicator, $"前往KOOK 私聊机器人注册");
                Helper.SendMessageToPeer(networkCommunicator, $"注册指令: /v {playerRegInfo.player_id.Replace("2.0.0.", "")} {playerRegInfo.verify_code}");
            }
        }
        catch (Exception e)
        {
            Helper.SendMessageToPeer(networkCommunicator, "欢迎来到BLMM服务器，注册码服务器异常，请在群里反馈- Q群:768296790");
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

    public bool CheckPlayerSelectPerks()
    {
        MissionScoreboardComponent missionScoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();
        MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = missionScoreboardComponent.GetSideSafe(BattleSideEnum.Attacker);
        MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide_defender = missionScoreboardComponent.GetSideSafe(BattleSideEnum.Attacker);
        if (missionScoreboardSide.CurrentPlayerCount < willMatchData.GetTeamMaxNum() || missionScoreboardSide_defender.CurrentPlayerCount < willMatchData.GetTeamMaxNum())
        {
            Helper.SendMessageToAllPeers("双方人数未到齐");
            return false;
        }
        return true;
    }

    public override async void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);


        if (timerIn3s.Check(Mission.Current.CurrentTime))
        {
            Helper.PrintError($"[ES]CurPlayerNum:{GameNetwork.NetworkPeerCount}, Spectator:{Mission.GetMissionBehavior<MissionScoreboardComponent>().Spectators.Count}");

            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(value: out string serverName);

            //Helper.Print($"willMatchData {willMatchData.isCancel} {willMatchData.isFinished}");
            // 比赛 已取消 或者已结束，立即获取信息
            if (willMatchData == null || willMatchData.isCancel || willMatchData.isFinished)
            {
                await NetUtil.GetAsync("/get-match-obj/" + serverName,
                (res) =>
                {
                    EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(res);
                    if (eWebResponse != null)
                    {
                        if (eWebResponse.code == -1)
                        {
                            Helper.PrintWarning($"[es|get-match-obj] {eWebResponse.Message}");
                        }
                        else
                        {
                            //Console.WriteLine(eWebResponse.data);
                            WillMatchData? willMatch = JsonConvert.DeserializeObject<WillMatchData>(eWebResponse.data.ToString());
                            if (willMatch == null)
                            {
                                Helper.PrintError("[es|get-match-obj] data is null");
                                return;
                            }

                            willMatchData = willMatch;
                            Helper.SendMessageToAllPeers("成功读取匹配数据");
                            //Console.WriteLine(willMatch.isCancel);
                            //Console.WriteLine(willMatch.firstTeamCultrue);
                            //foreach (string item in willMatch.secondTeamPlayerIds)
                            //{
                            //    Console.WriteLine(item);
                            //}
                        }
                    }
                }, (e) =>
                {
                    Helper.Print("[es|get-match-obj] error");
                    Helper.Print(e.Message);
                    Helper.Print(e.StackTrace);
                });
            }

            if (!IsRegEvent)
            {
                MultiplayerRoundController multiplayerRoundController = Mission.GetMissionBehavior<MultiplayerRoundController>();
                if (multiplayerRoundController == null)
                {
                    Helper.Print("[es|event]register event failed");
                    return;
                }
                else
                {
                    //multiplayerRoundController.OnPostRoundEnded += SendData;
                    //multiplayerRoundController.OnPreRoundEnding += SendData;
                    //multiplayerRoundController.OnRoundEnding += SendData;
                    //multiplayerRoundController.OnCurrentRoundStateChanged += SendData;

                    Helper.Print("[es|event]register event successful");
                    IsRegEvent = true;
                }
            }
            else
            {
            }
        }

        if (timerIn5s.Check(Mission.CurrentTime))
        {
            Helper.SendMessageToAllPeers($"剩余时间:{MaxAfterPlayerArrivadedWatingCount - AfterPlayerArrivadedWatingCount}");

            if (CheckPlayerSelectPerks())
            {
                MultiplayerWarmupComponent multiplayerWarmupComponent = Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
                multiplayerWarmupComponent.EndWarmupProgress();
            }
            else
            {
                MarkCurrentMatchCancel();
            }

        }
        if (timerIn1s.Check(Mission.Current.CurrentTime))
        {
            AfterPlayerArrivadedWatingCount += 1;

            MultiplayerWarmupComponent multiplayerWarmupComponent = Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
            if (multiplayerWarmupComponent != null && multiplayerWarmupComponent.IsInWarmup)
            {
                if (MaxAfterPlayerArrivadedWatingCount - AfterPlayerArrivadedWatingCount < 10)
                {
                    Helper.SendMessageToAllPeers($"剩余时间:{MaxAfterPlayerArrivadedWatingCount - AfterPlayerArrivadedWatingCount}");
                }
            }
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

        string playerId = Helper.GetPlayerId(networkPeer);

        if (willMatchData != null && willMatchData.isplayerNeedMatch(playerId))
        {
            willMatchData.OffSetCurrentPlayerNumber(-1);
            Helper.SendMessageToAllPeers($"当前到场人数 {willMatchData.CurrentPlayerNumber}/{willMatchData.GetTotalNumber()}");
        }
    }


    public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
    {
        string playerId = networkPeer.VirtualPlayer.Id.ToString();
        //Helper.SendMessageToPeer(networkPeer, playerId);
        Helper.SendMessageToAllPeers($"欢迎 {networkPeer.UserName} 进入服务器");
        Helper.SendMessageToPeer(networkPeer, "本BLMM服务器由C_urse 赞助，快说感谢大C!");

        dataContainer.AddPlayerWithName(networkPeer);
        //Debug.Print(JsonConvert.SerializeObject(dataContainer));
        GetPlayerVerifyCodeAsync(networkPeer);
        SendPlayerName(networkPeer);

        Debug.Print("[OnPlayerConnectedToServer|PlayerJoined]");

        KickWeirdBodyProperties(networkPeer);
        KickEmptyNames(networkPeer);

        if (willMatchData != null && willMatchData.isplayerNeedMatch(playerId))
        {
            willMatchData.OffSetCurrentPlayerNumber(+1);
            Helper.SendMessageToAllPeers($"当前到场人数 {willMatchData.CurrentPlayerNumber}/{willMatchData.GetTotalNumber()}");

            if (willMatchData.isPlayerArrived())
            {
                Helper.SendMessageToAllPeers($"比赛双方到齐，请尽快进入对应的队伍");
            }
        }
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

                if (affectedAgent.Team == affectorAgent.Team)
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
                    if (affectedRiderAgent.Team == affectorAgent.Team)
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

        MultiplayerWarmupComponent multiplayerWarmupComponent = Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
        if (multiplayerWarmupComponent == null || !multiplayerWarmupComponent.IsInWarmup)
        {
            SendData();
        }
        else
        {
            Helper.Print("[es|OnEndMission] in warmp up ,dont send data");
        }

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
            string affectedPlayerId = Helper.GetPlayerId(affectedAgent);
            string affectedPlayerName = affectedAgent.MissionPeer.Name;

            if (affectorAgent.IsHuman && affectorAgent.IsPlayerControlled)
            {
                string affectorPlayerId = Helper.GetPlayerId(affectorAgent);

                if (affectorAgent.Team.IsEnemyOf(affectedAgent.Team))
                {
                    KeyValuePair<bool, int> k = dataContainer.AddKillRecord(affectorPlayerId, affectedPlayerId);
                    if (k.Key)
                    {
                        switch (k.Value)
                        {
                            case 2:
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                break;
                            case 3:
                            case 4:
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                break;
                            case 5:
                            case 6:
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 连杀");
                                break;
                            case 7:
                            case 8:
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 已超神");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 已超神");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 已超神");
                                break;
                            case 9:
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 已杀穿对面");
                                Helper.SendMessageToAllPeers($"玩家 {affectedPlayerName} {k.Value} 已杀穿对面");
                                break;
                        }
                    }
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


    public async void SendData()
    {
        MultiplayerRoundController multiplayerRoundController = Mission.GetMissionBehavior<MultiplayerRoundController>();
        try
        {
            if (multiplayerRoundController == null)
            {
                Helper.PrintError("[es|multiplayerRoundController is null]");
                return;
            }

            MissionScoreboardComponent scoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();

            int attacker_score = scoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
            foreach (MissionPeer missionPeer in scoreboardComponent.GetSideSafe(BattleSideEnum.Attacker).Players)
            {
                string PlayerId = missionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                dataContainer.SetKillNumber(PlayerId, missionPeer.KillCount);
                dataContainer.SetDeathNumber(PlayerId, missionPeer.DeathCount);
                dataContainer.SetAssistNumber(PlayerId, missionPeer.AssistCount);
                // 添加进攻队伍
                PlayerMatchDataContainer.AddAttackPlayer(PlayerId);
                dataContainer.SetRoundScore(PlayerId, attacker_score);

            }

            int defender_score = scoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
            foreach (MissionPeer missionPeer in scoreboardComponent.GetSideSafe(BattleSideEnum.Defender).Players)
            {
                string PlayerId = missionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
                dataContainer.SetKillNumber(PlayerId, missionPeer.KillCount);
                dataContainer.SetDeathNumber(PlayerId, missionPeer.DeathCount);
                dataContainer.SetAssistNumber(PlayerId, missionPeer.AssistCount);
                // 添加防御队伍
                PlayerMatchDataContainer.AddDefendPlayer(PlayerId);
                dataContainer.SetRoundScore(PlayerId, defender_score);
            }

            Dictionary<string, object> data = new()
            {
                ["IsMatchEnding"] = multiplayerRoundController.IsMatchEnding,
                ["RoundCount"] = multiplayerRoundController.RoundCount,
                ["MatchTime"] = DateTime.Now.ToString()
            };
            PlayerMatchDataContainer.SetTag(data);

        }
        catch (Exception e)
        {
            Helper.PrintError(e.Message);
            Helper.PrintError(e.StackTrace ?? "null");
            return;
        }

        ////////////////////////////////////////
        // 发送数据
        ////////////////////////////////////////
        //Helper.Print("[es|send data to web]");
        ////PlayerMatchDataContainer.TurnMatchToNorm();
        //string result = JsonConvert.SerializeObject(dataContainer);
        //Debug.Print(result);
        //try
        //{
        //    HttpHelper.PostStringAsync(WebUrlManager.UploadMatchData, result);
        //    Debug.Print("[es|Sended DataToServer]");

        //}
        //catch (Exception ex)
        //{
        //    Debug.Print(ex.Message, color: Debug.DebugColor.Red);
        //    Debug.Print(ex.StackTrace, color: Debug.DebugColor.Red);
        //    return;
        //}
        // 刷新数据
        //dataContainer.RefreshhAll();

        // 踢出玩家
        if (multiplayerRoundController.IsMatchEnding)
        {
            Helper.Print("[es| match over , kick out all man]");
            Helper.SendMessageToAllPeers("比赛结束。将踢出所有人");
            await Task.Delay(2000);
            KickHelper.KickList(GameNetwork.NetworkPeers);
        }
    }

    public override void OnClearScene()
    {
        //if (k == null)
        //{
        //    Debug.Print("OnClearScene|IS NULL");
        //}
        //else
        //{
        //    Debug.Print($"OnClearScene|{k.IsInWarmup}");
        //}
    }

}
