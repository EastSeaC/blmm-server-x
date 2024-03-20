using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Library;

namespace BLMMX.ConstanSet
{
    public class WebUrlManager
    {
        private const string host = "localhost";
        private const string prot = "10072";
        private const string url = $"http://{host}:{prot}/game/";
        public WebUrlManager() { }

        private const string _getKickList = "GetKickList";
        private const string _updatePlayerNumber = "UpdateCurrentPlayerNumber";

        private const string _getPlayerList = "GetPlayerList";
        public static string GetPlayerList = url + _getPlayerList;

    }
}
