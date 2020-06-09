using Caf.MongoDB.Uow;
using Caf.UnitOfWork.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoDbContextProvider<TMongoDbContext> : IMongoDbContextProvider<TMongoDbContext>
        where TMongoDbContext : ICafMongoDbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUowHelper _uowHelper;
        public MongoDbContextProvider(IServiceProvider serviceProvider, IUowHelper uowHelper)
        {
            _uowHelper = uowHelper;
            _serviceProvider = serviceProvider;
        }
        public TMongoDbContext GetDbContext()
        {
            //是否存在工作单元
            if (_uowHelper.IsExistUow)
            {
                var key = typeof(TMongoDbContext).FullName;
                if (_uowHelper.IsExistConnectionKey(key))
                {
                    var efuowConnection = (MongoDbUOWConnection)_uowHelper.GetOrAddUOWConnection(key, null);
                    return (TMongoDbContext)efuowConnection.DbContext;
                }
                var dbcontext = (ICafMongoDbContext)_serviceProvider.GetRequiredService(typeof(TMongoDbContext));
                var dbtransaction = dbcontext.mongoClient.StartSession();
                _uowHelper.GetOrAddUOWConnection(key, new MongoDbUOWConnection { DbContext = dbcontext, ClientSession = dbtransaction });
                return (TMongoDbContext)dbcontext;
            }
            else
            {
                return (TMongoDbContext)_serviceProvider.GetRequiredService(typeof(TMongoDbContext));
            }
        }
    }
}
