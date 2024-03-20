using System;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Client
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            Module.CurrentModule.AddMultiplayerGameMode(new BLMM());
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
        }


    }
}
