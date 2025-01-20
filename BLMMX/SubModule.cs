using BLMMX.ChatCommands.PlayerCommands;
using BLMMX.ChatCommands;
using BLMMX.Handler;
using BLMMX.Helpers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using BLMMX.ChatCommands.AdminCommands;
using HarmonyLib;
using BLMMX.Patch;

namespace BLMMX;

public class SubModule : MBSubModuleBase
{
    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

        Helper.Print("Registering Chat Commands...");
        RegisterChatCommands();

        Harmony harmony = new("com.es.patch");
        harmony.PatchAll();
        PatchWorker.Apply(harmony);
    }

    private static void RegisterChatCommands()
    {
        ChatCommandHandler commandHandler = ChatCommandHandler.Instance;

        commandHandler.RegisterCommand(new PlayerBasicCommand());
        commandHandler.RegisterCommand(new ShowMapsCommand());
        commandHandler.RegisterCommand(new SetMapCommand());
        commandHandler.RegisterCommand(new CancelMatchCommand());
        commandHandler.RegisterCommand(new StartNewMapCommand());
        commandHandler.RegisterCommand(new ChangeSocreCommand());
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);
        mission.AddMissionBehavior(new BLMMBehavior2());
        Debug.Print("[OnMissionBehaviorInitialize|已启动]");

    }

    public override void OnMultiplayerGameStart(Game game, object starterObject)
    {
        base.OnMultiplayerGameStart(game, starterObject);

        Helper.Print("Adding GameHandler.");
        // 不使用Handler,当前没办法控制其覆盖PE的处理器
        game.AddGameHandler<ESHandler>();
    }

    public override void OnBeforeMissionBehaviorInitialize(Mission mission)
    {
        base.OnBeforeMissionBehaviorInitialize(mission);
    }
}
