using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoDbContextModel
    {
        public IReadOnlyDictionary<Type, IMongoEntityModel> Entities { get; }

        public MongoDbContextModel(IReadOnlyDictionary<Type, IMongoEntityModel> entities)
        {
            Entities = entities;
        }
    }
}
