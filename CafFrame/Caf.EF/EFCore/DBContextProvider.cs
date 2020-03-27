using Caf.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.EF.EFCore
{
    public class DbContextProvider<EntityType>: IDbContextProvider<EntityType> where EntityType:class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUowHelper _uowHelper;
        private readonly Type dbContextType;
        public DbContextProvider(IServiceProvider serviceProvider, IUowHelper uowHelper, IEFDbContextEntityMapping efDbContextEntityMapping)
        {
            _uowHelper = uowHelper;
            _serviceProvider = serviceProvider;
            dbContextType = efDbContextEntityMapping.GetDbContextByEntity<EntityType>();
             
        }

        public DbContext GetDbContext()
        {
            //是否存在工作单元
            if(_uowHelper.IsExistUow)
            {
                var key = dbContextType.FullName;
                if(_uowHelper.IsExistConnectionKey(key))
                {
                   var efuowConnection = (EFUOWConnection)_uowHelper.GetOrAddUOWConnection(key, null);
                    return efuowConnection.dbContext;
                }
                var dbcontext = (DbContext)_serviceProvider.GetRequiredService(dbContextType);
                var dbtransaction = dbcontext.Database.BeginTransaction();
                _uowHelper.GetOrAddUOWConnection(key, new EFUOWConnection { dbContext= dbcontext, DbContextTransaction = dbtransaction });
                return dbcontext;
            }
            else
            {
                return (DbContext)_serviceProvider.GetRequiredService(dbContextType);
            }
        }
    }
}
