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

                MethodInfo? methodInfo = typeof(MultiplayerWarmupComponent).GetMethod("EndWarmup", BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(multiplayerWarmupComponent, null); // 第二个参数是方法的参数，若无则传递 null
                }
                else
                {
                    Helper.SendMessageToPeer(executor, "无法调用");
                }
                return true;
            }
            return false;
        }
    }
}
