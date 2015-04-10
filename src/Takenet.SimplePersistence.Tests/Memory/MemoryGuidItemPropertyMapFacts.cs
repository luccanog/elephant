﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.SimplePersistence.Memory;

namespace Takenet.SimplePersistence.Tests.Memory
{
    public class MemoryGuidItemPropertyMapFacts : GuidItemPropertyMapFacts
    {
        public override IPropertyMap<Guid, Item> Create()
        {
            return new Map<Guid, Item>();
        }
    }
}
