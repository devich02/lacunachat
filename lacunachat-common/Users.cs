﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lacuna;

namespace lacunachat
{
    namespace tables
    {
        public class Users
        {
            [DbColumn] public String Name { get; set; }
            [DbColumn] public String Key { get; set; }
        }
    }
}
