using Caf.Core.Module;
using Caf.Domain;
using Caf.MongoDB.Repository;
using Caf.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    [UsingModule(typeof(CafDomainModule))]
    [UsingModule(typeof(UowModule))]
    public class CafMongoDbModule : CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton<MongoDBContextSource>(new MongoDBContextSource());
            context.Services.TryAddTransient(
                typeof(IMongoDbContextProvider<>),
                typeof(MongoDbContextProvider<>)
            );
            context.Services.AddScoped(typeof(IMongoDbRepository<,>), typeof(MongoDbRepository<,>));
            context.Services.AddScoped(typeof(IMongoDbRepository<,,>), typeof(MongoDbRepository<,,>));
        }
    }
}
