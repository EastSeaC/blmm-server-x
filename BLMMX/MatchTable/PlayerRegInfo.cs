using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX.MatchTable;

public class PlayerRegInfo
{
    public bool is_reg { get; set; }
    public string verify_code { get; set; }
    public string player_id { get; set; }

    // 默认构造函数
    public PlayerRegInfo()
    {
    }

    // 带参数的构造函数
    public PlayerRegInfo(bool isReg, string verifyCode, string playerId)
    {
        is_reg = isReg;
        verify_code = verifyCode;
        player_id = playerId;
    }

    // 重写ToString方法以便于打印对象
    public override string ToString()
    {
        return $"IsReg: {is_reg}, VerifyCode: {verify_code}, PlayerId: {player_id}";
    }
}
