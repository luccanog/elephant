﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.SimplePersistence.Tests
{
    public abstract class SetMapFacts<TKey, TValue> : MapFacts<TKey, ISet<TValue>>
    {
    }
}
