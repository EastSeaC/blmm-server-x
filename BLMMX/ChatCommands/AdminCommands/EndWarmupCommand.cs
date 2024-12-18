using BLMMX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BLMMX.ChatCommands.AdminCommands
{
    public class EndWarmupCommand : AdminChatCommand
    {
        public override string CommandText => "endwarm";

        public override string Description => "";


        public override bool Execute(NetworkCommunicator executor, string args)
        {
            if (Mission.Current != null)
            {
                MultiplayerWarmupComponent multiplayerWarmupComponent = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();
                multiplayerWarmupComponent.EndWarmupProgress();


                Helper.SendMessageToAllPeers("热身结束");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
