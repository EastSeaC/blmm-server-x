using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Multiplayer;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace BLMMX
{
    internal class BLMM : MissionBasedMultiplayerGameMode
    {
        private const string GameName = "BLMMX";
        public BLMM() : base(GameName)
        {
        }

        [MissionMethod]
        public override void StartMultiplayerGame(string scene)
        {
            base.StartMultiplayerGame(scene);

            MissionState.OpenNew(GameName, new MissionInitializerRecord(scene), delegate (Mission missionController)
            {
                if (GameNetwork.IsServer)
                {
                    return new MissionBehavior[]
                    {
                        MissionLobbyComponent.CreateBehavior(),
                        new MissionMultiplayerFlagDomination(MultiplayerGameType.Skirmish),
                        new MultiplayerRoundController(),
                        new MultiplayerWarmupComponent(),
                        new MissionMultiplayerGameModeFlagDominationClient(),
                        new MultiplayerTimerComponent(),
                        new SpawnComponent(new FlagDominationSpawnFrameBehavior(), new FlagDominationSpawningBehavior()),
                        new MissionLobbyEquipmentNetworkComponent(),
                        new MultiplayerTeamSelectComponent(),
                        new MissionHardBorderPlacer(),
                        new MissionBoundaryPlacer(),
                        new AgentVictoryLogic(),
                        new MissionAgentPanicHandler(),
                        new AgentHumanAILogic(),
                        new MissionBoundaryCrossingHandler(),
                        new MultiplayerPollComponent(),
                        new MultiplayerAdminComponent(),
                        new MultiplayerGameNotificationsComponent(),
                        new MissionOptionsComponent(),
                        new MissionScoreboardComponent(new SkirmishScoreboardData()),
                        new EquipmentControllerLeaveLogic(),
                        new VoiceChatHandler(),
                        new MultiplayerPreloadHelper(),

                        new BLMMBehavior2(),
                    };
                }
                return new MissionBehavior[]
                {
                    MissionLobbyComponent.CreateBehavior(),
                    new MultiplayerAchievementComponent(),
                    new MultiplayerWarmupComponent(),
                    new MissionMultiplayerGameModeFlagDominationClient(),
                    new MultiplayerRoundComponent(),
                    new MultiplayerTimerComponent(),
                    new MissionLobbyEquipmentNetworkComponent(),
                    new MultiplayerTeamSelectComponent(),
                    new MissionHardBorderPlacer(),
                    new MissionBoundaryPlacer(),
                    new MissionBoundaryCrossingHandler(),
                    new MultiplayerPollComponent(),
                    new MultiplayerAdminComponent(),
                    new MultiplayerGameNotificationsComponent(),
                    new MissionOptionsComponent(),
                    new MissionScoreboardComponent(new SkirmishScoreboardData()),
                    MissionMatchHistoryComponent.CreateIfConditionsAreMet(),
                    new EquipmentControllerLeaveLogic(),
                    new MissionRecentPlayersComponent(),
                    new VoiceChatHandler(),
                    new MultiplayerPreloadHelper()
                };
            }, true, true);
        }

    }
}