﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.SimplePersistence.Redis;
using Xunit;

namespace Takenet.SimplePersistence.Tests.Redis
{
    [Collection("Redis")]
    public class RedisHashGuidItemPropertyMapFacts : GuidItemPropertyMapFacts
    {
        private readonly RedisFixture _redisFixture;

        public RedisHashGuidItemPropertyMapFacts(RedisFixture redisFixture)
        {
            _redisFixture = redisFixture;
        }

        public override IPropertyMap<Guid, Item> Create()
        {
            _redisFixture.Server.FlushDatabase();
            const string mapName = "guid-item-hash";
            return new RedisHashMap<Guid, Item>(mapName, new ItemDictionaryConverter(), _redisFixture.Connection.Configuration);
        }
    }
}
