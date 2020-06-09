using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface IMongodbClientFactory
    {
        MongoClient GetClient(string connectionStr);
    }
}
