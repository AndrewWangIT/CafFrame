using Caf.MongoDB.Uow;
using Caf.UnitOfWork.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoDbContextProvider<EntityType> : IMongoDbContextProvider<EntityType> where EntityType : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUowHelper _uowHelper;
        private readonly Type dbContextType;
        public MongoDbContextProvider(IServiceProvider serviceProvider, IUowHelper uowHelper, IMongoContextEntityMapping  mongoContextEntityMapping)
        {
            _uowHelper = uowHelper;
            _serviceProvider = serviceProvider;
            dbContextType = mongoContextEntityMapping.GetDbContextByEntity<EntityType>();
        }
        public ICafMongoDbContext GetDbContext()
        {
            //是否存在工作单元
            if (_uowHelper.IsExistUow)
            {
                var key = dbContextType.FullName;
                if (_uowHelper.IsExistConnectionKey(key))
                {
                    var mongouowConnection = (MongoDbUOWConnection)_uowHelper.GetOrAddUOWConnection(key, null);
                    return (ICafMongoDbContext)mongouowConnection.DbContext;
                }
                var dbcontext = (ICafMongoDbContext)_serviceProvider.GetRequiredService(dbContextType);
                var dbtransaction = dbcontext.mongoClient.StartSession();
                _uowHelper.GetOrAddUOWConnection(key, new MongoDbUOWConnection { DbContext = dbcontext, ClientSession = dbtransaction });
                return (ICafMongoDbContext)dbcontext;
            }
            else
            {
                return (ICafMongoDbContext)_serviceProvider.GetRequiredService(dbContextType);
            }
        }
    }
}
