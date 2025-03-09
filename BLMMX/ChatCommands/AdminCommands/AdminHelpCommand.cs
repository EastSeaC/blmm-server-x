using BLMMX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    internal class AdminHelpCommand : AdminChatCommand
    {
        public override string CommandText => "help";

        public override string Description => "指令说明";

        public override bool Execute(NetworkCommunicator executor, string args)
        {
            Helper.SendMessageToPeer(executor, new ChangeSocreCommand().Description);
            return true;
        }
    }
}
