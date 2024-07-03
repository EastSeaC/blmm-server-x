using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BLMMX;

public class SubModule : MBSubModuleBase
{
    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);
        mission.AddMissionBehavior(new BLMMBehavior2());
        Debug.Print("[OnMissionBehaviorInitialize|已启动]");

    }

    public override void OnBeforeMissionBehaviorInitialize(Mission mission)
    {
        base.OnBeforeMissionBehaviorInitialize(mission);
    }
}
