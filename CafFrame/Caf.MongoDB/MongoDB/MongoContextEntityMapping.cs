using Caf.Core;
using Caf.Core.Caf.Utils.Reflection;
using Caf.Core.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoContextEntityMapping : IMongoContextEntityMapping, ISingleton
    {
        private Dictionary<Type, Type> _maps { get; set; } = new Dictionary<Type, Type>();
        public Type GetDbContextByEntity<TEntity>()
        {
            var entityType = typeof(TEntity);
            if (!_maps.ContainsKey(entityType))
            {
                throw new CafException($"该实体{entityType.Name}未包含在DbContext中");
            }
            return _maps[entityType];
        }

        public void LoadMaping(Type dbContextType)
        {
            var dbquery = dbContextType.GetTypeInfo().Assembly.GetTypes().Where(o => o.CanAssignableTo(typeof(ICafMongoDbContext)));

            foreach (var dbtype in dbquery)
            {
                var entitys = from property in dbtype.GetTypeInfo()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              where
                              property.PropertyType.CanAssignableTo(typeof(IMongoCollection<>))
                              select property.PropertyType.GenericTypeArguments[0];
                foreach (var entityType in entitys)
                {
                    if (_maps.ContainsKey(entityType))
                    {
                        throw new CafException($"存在一个实体对应多个DbContext,entityType:{entityType.FullName}");
                    }
                    _maps.Add(entityType, dbtype);
                }
            }
        }
    }
}
