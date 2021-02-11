﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementEngine.ECS
{
    public class View
    {
        public Type[] Types;
        public SparseSet Entities = new SparseSet(1000);

        public View(Type[] types)
        {
            Types = new Type[types.Length];
            types.CopyTo(Types, 0);
        }
    }
}
