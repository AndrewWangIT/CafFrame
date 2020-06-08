using Caf.MongoDB.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongodbTest
{
    public class ContentLibraryDbContext : CafMongoDbContext
    {
        public ContentLibraryDbContext(IMongoModelSource modelSource, MongoDBContextSource mongoDBContextSource, IMongodbClientFactory mongodbClientFactory) : base(modelSource, mongoDBContextSource, mongodbClientFactory)
        {

        }
        public IMongoCollection<UserBehavior> userBehaviors { get; set; }
    }
}
