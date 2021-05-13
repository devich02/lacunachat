using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lacuna;

namespace lacunachat
{
    namespace tables
    {
        public class Messages
        {
            [DbColumn] public String From { get; set; }
            [DbColumn] public String To { get; set; }
            [DbColumn] public String Message { get; set; }
        }
    }
}
