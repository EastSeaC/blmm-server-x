using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX
{
    internal enum ESMatchSate
    {
        None = 0,
        // 等待玩家进入
        Wait = 1,
        // 开始
        Started = 2,
        End = 3,

        // 当收到服务器传来的玩家信息时，立即进入 Wait
        PlayerStateReceiving = 4,
    }
}
