using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX.Helpers;

public class AdminManager
{
    public static List<string> admins { get; set; } = new();

    public static bool IsAdmin(string id)
    {
        return admins.Contains(id);
    }
}
