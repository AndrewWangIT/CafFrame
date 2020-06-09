using Caf.Core.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoDBContextSource
    {
        protected readonly ConcurrentDictionary<Type, MongoDBContextOptions> MetadataCache = new ConcurrentDictionary<Type, MongoDBContextOptions>();
        public void Load(Type contextType,Action<MongoDBContextOptions> action)
        {
            MongoDBContextOptions metadata = new MongoDBContextOptions();
            var databaseAttribute = contextType.CustomAttributes.OfType<MongoDatabaseAttribute>().FirstOrDefault();
            var databaseName = databaseAttribute?.DatabaseName ?? "";
            metadata.DatabaseName = databaseName;
            action?.Invoke(metadata);
            MetadataCache.GetOrAdd(contextType, metadata);
        }
        public MongoDBContextOptions GetDbOption(Type contextType)
        {
            return MetadataCache.GetValueOrDefault(contextType);
        }
    }
    public class MongoDBContextOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
