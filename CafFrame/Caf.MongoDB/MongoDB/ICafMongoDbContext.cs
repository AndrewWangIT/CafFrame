using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface ICafMongoDbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<T> Collection<T>();

        MongoClient mongoClient { get; }
    }
}
