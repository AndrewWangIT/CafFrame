using Caf.Core.Module;
using Caf.MongoDB.Extensions;
using Caf.MongoDB.MongoDB;
using System;

namespace MongodbTest
{
    [UsingModule(typeof(CafMongoDbModule))]
    public class MongodbTestMoudle : CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            var connectionString = "mongodb://49.234.12.187:27017";

            context.Services.AddMongoDbContext<ContentLibraryDbContext>(options =>
            {
                options.ConnectionString = connectionString;
                options.DatabaseName = "UserBehavior";
            });
        }
    }
}
