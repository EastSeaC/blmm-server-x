using TaleWorlds.MountAndBlade;

namespace BLMMX
{
    internal class BLMMSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            //Module.CurrentModule.AddMultiplayerGameMode(new BLMM());
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);

        }
    }


}
