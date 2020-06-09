using Caf.MongoDB.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.Extensions
{
    public static class AbpMongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TMongoDbContext>(this IServiceCollection services, Action<MongoDBContextOptions> optionsBuilder = null) //Created overload instead of default parameter
            where TMongoDbContext : CafMongoDbContext
        {
            var source = services.GetSingletonInstance<MongoDBContextSource>();
            source.Load(typeof(TMongoDbContext), optionsBuilder);
            return services;
        }
    }
}
