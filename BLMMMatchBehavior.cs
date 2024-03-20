//using BLMMX.ConstanSet;
//using Newtonsoft.Json;
//using TaleWorlds.Core;
//using TaleWorlds.Library;
//using TaleWorlds.MountAndBlade;
//using TaleWorlds.ObjectSystem;
//using Timer = TaleWorlds.Core.Timer;

//namespace BLMMX
//{
//    public class BLMMMatchBehavior : MultiplayerTeamSelectComponent
//    {
//        private Timer? PlayerCheck;
//        private ESMatchSate eSMatchSate = ESMatchSate.Wait;

//        private static List<string> AttackPlayerIds;
//        private static List<string> DefendPlayerIds;

//        public override void EarlyStart()
//        {
//            base.EarlyStart();

//        }

//        public override void OnBehaviorInitialize()
//        {
//            base.OnBehaviorInitialize();

//            PlayerCheck = new Timer(Mission.CurrentTime, 0.2f);
//            Debug.Print(eSMatchSate.ToString());
//        }

//        public override void OnMissionTick(float dt)
//        {
//            base.OnMissionTick(dt);

//            if (PlayerCheck != null)
//            {
//                if (PlayerCheck.Check(Mission.CurrentTime))
//                {
//                    Debug.Print(eSMatchSate.ToString());
//                    //BK();
//                }
//            }
//        }


//        public void BK()
//        {
//            if (eSMatchSate == ESMatchSate.PlayerStateReceiving)
//            {
//                if (AttackPlayerIds != null && DefendPlayerIds != null)
//                {
//                    eSMatchSate = ESMatchSate.Wait;
//                }
//                //UpdatePlayerListAsync();
//            }
//            else if (eSMatchSate == ESMatchSate.Wait)
//            {
//                BasicCultureObject cultureTeam1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
//                BasicCultureObject cultureTeam2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
//                // 等待玩家加入
//                foreach (NetworkCommunicator? peer in GameNetwork.NetworkPeers)
//                {
//                    string PlayerId = peer.VirtualPlayer.Id.ToString();
//                    //MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
//                    if (AttackPlayerIds.Contains(PlayerId))
//                    {
//                        ChangeTeamServer(peer, Mission.AttackerTeam);
//                    }
//                    else if (DefendPlayerIds.Contains(PlayerId))
//                    {
//                        ChangeTeamServer(peer, Mission.DefenderTeam);
//                    }
//                    else
//                    {
//                        ChangeTeamServer(peer, Mission.SpectatorTeam);
//                    }
//                }

//                int attack_num = 0;
//                int defender_num = 0;
//                foreach (NetworkCommunicator peer in GameNetwork.NetworkPeers)
//                {
//                    MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
//                    if (missionPeer != null)
//                    {
//                        if (missionPeer.Team == Mission.AttackerTeam)
//                        {
//                            attack_num++;
//                        }
//                        else if (missionPeer.Team == Mission.DefenderTeam)
//                        {
//                            defender_num++;
//                        }
//                    }
//                }

//                if (attack_num == AttackPlayerIds.Count && defender_num == DefendPlayerIds.Count)
//                {

//                }
//            }
//            else if (eSMatchSate == ESMatchSate.Started)
//            {

//            }
//        }

//        //public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
//        //{
//        //    if (prevTeam == newTeam)
//        //    {
//        //        base.OnAgentTeamChanged(prevTeam, newTeam, agent);
//        //    }
//        //    else
//        //    {
//        //        return;
//        //    }
//        //}
//        public async void UpdatePlayerListAsync()
//        {
//            string result;
//            try
//            {
//                result = await HttpHelper.DownloadStringTaskAsync(WebUrlManager.GetPlayerList);
//                if (result != null && result.IsEmpty())
//                {
//                    TeamInfo strings = JsonConvert.DeserializeObject<TeamInfo>(result);
//                    AttackPlayerIds = strings.AttackPlayerIds;
//                    DefendPlayerIds = strings.DefenderPlayerIds;
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.Print(ex.Message);
//                return;
//            }

//        }
//    }
//}