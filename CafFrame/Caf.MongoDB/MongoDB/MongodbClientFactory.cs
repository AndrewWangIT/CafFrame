using Caf.Core.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongodbClientFactory : IMongodbClientFactory, ISingleton
    {
        private readonly ConcurrentDictionary<string, MongoClient> clientdic = new ConcurrentDictionary<string, MongoClient>();
        public MongoClient GetClient(string connectionStr)
        {
            return clientdic.GetOrAdd(
                connectionStr,
                _ => new MongoClient(connectionStr)
            );
        }
    }
}
