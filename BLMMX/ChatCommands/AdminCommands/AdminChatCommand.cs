﻿using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands;

public abstract class AdminChatCommand: ChatCommand
{
    public override bool CanExecute(NetworkCommunicator executor)
    {
        return executor.IsAdmin;
    }
}