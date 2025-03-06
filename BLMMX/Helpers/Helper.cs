using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BLMMX.Helpers
{
    public static class Helper
    {
        private const string Prefix = "[ES] ";
        private const string WarningPrefix = Prefix + "[WARN] ";
        private const string ErrorPrefix = "[ERROR] ";

        public static void Print(string message) =>
            Debug.Print(Prefix + message, 0, Debug.DebugColor.DarkGreen);


        public static void PrintWarning(string message) =>
            Debug.Print(WarningPrefix + message, 0, Debug.DebugColor.DarkYellow);


        public static void PrintError(string message) =>
            Debug.Print(ErrorPrefix + message, 0, Debug.DebugColor.DarkRed);

        public static void SendMessageToAllPeers(string message)
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new ServerMessage(Prefix + message));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients);
        }

        public static void SendMessageToPeer(NetworkCommunicator peer, string message)
        {
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(new ServerMessage(Prefix + message));
            GameNetwork.EndModuleEventAsServer();
        }

        public static string GetPlayerId(NetworkCommunicator peer)
        {
            return peer.VirtualPlayer.Id.ToString();
        }

        public static string GetPlayerId(MissionPeer peer)
        {
            return peer.GetNetworkPeer().VirtualPlayer.Id.ToString();
        }

        public static string GetPlayerId(Agent agent)
        {
            if (agent.IsPlayerControlled)
            {
                return agent.MissionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
            }
            else
            {
                Print("[GetPlayerId|Error] No player");
                return "";
            }
        }

        public static bool HasBowOrCrossbow(this Agent agent)
        {
            for (EquipmentIndex equipmentIndex = EquipmentIndex.Weapon0; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                if (agent.Equipment[equipmentIndex].Item != null)
                {
                    WeaponComponentData primaryWeapon = agent.Equipment[equipmentIndex].Item.PrimaryWeapon;
                    if (primaryWeapon.IsBow || primaryWeapon.IsCrossBow)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}