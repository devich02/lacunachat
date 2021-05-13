using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lacuna;
using Newtonsoft.Json.Linq;

namespace lacunachat
{
    namespace tables
    {
        public class Users
        {
            [DbColumn] public String Name { get; set; }
            [DbColumn] public String Key { get; set; }
            [DbColumn] public String DecryptKey { get; set; }
            [DbColumn] public long CreateTimeUtc { get; set; }
            [DbColumn] public JToken Friends { get; set; }
        }

        public class AccessTokens
        {
            [DbColumn] public long TokenId { get; set; }
            [DbColumn] public long CreateTimeUtc { get; set; }
            [DbColumn] public long ExpireTimeUtc { get; set; }
            [DbColumn] public bool Expired { get; set; }
        }
    }
}
