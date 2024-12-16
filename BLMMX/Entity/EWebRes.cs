using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX.Entity
{
    public class EWebResponse
    {
        public EWebResponse() { }

        public string Message { get; set; }

        public int code{ get; set; }

        public object data { get; set; }
    }
}
