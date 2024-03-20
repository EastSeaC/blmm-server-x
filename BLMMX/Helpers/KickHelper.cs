using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;

namespace BLMMX.Helpers;

public class KickHelper
{
    public static void Kick(NetworkCommunicator networkPeer)
    {
        if (networkPeer == null)
        {
            DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
            disconnectInfo.Type = DisconnectType.KickedByHost;
            networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
            GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
        }
    }

    public static void Kick(NetworkCommunicator networkPeer, DisconnectType kickedByHost, string v)
    {
        DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
        disconnectInfo.Type = kickedByHost;
        networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
        GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
    }
}
