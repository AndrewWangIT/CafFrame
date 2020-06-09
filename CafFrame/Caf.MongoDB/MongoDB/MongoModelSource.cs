using Caf.Core.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Caf.Domain.Entities;
using Caf.Core.Caf.Utils.Reflection;

namespace Caf.MongoDB.MongoDB
{
    public class MongoModelSource : IMongoModelSource, ISingleton
    {
        protected readonly ConcurrentDictionary<Type, MongoDbContextModel> ModelCache = new ConcurrentDictionary<Type, MongoDbContextModel>();
        public MongoDbContextModel GetModel(CafMongoDbContext dbContext)
        {
            return ModelCache.GetOrAdd(
                dbContext.GetType(),
                _ => CreateModel(dbContext)
            );
        }

        private MongoDbContextModel CreateModel(CafMongoDbContext dbContext)
        {
            var modelBuilder = new MongoModelBuilder();
            BuildModelFromDbContextType(modelBuilder, dbContext.GetType());
            BuildModelFromDbContextInstance(modelBuilder, dbContext);
            return modelBuilder.Build();
        }
        protected virtual void BuildModelFromDbContextType(MongoModelBuilder modelBuilder, Type dbContextType)
        {
            var collectionProperties =
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionExt.CanAssignableTo(property.PropertyType, typeof(IMongoCollection<>)) &&
                    typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property;

            foreach (var collectionProperty in collectionProperties)
            {
                BuildModelFromDbContextCollectionProperty(modelBuilder, collectionProperty);
            }
        }

        protected virtual void BuildModelFromDbContextCollectionProperty(MongoModelBuilder modelBuilder, PropertyInfo collectionProperty)
        {
            var entityType = collectionProperty.PropertyType.GenericTypeArguments[0];
            var collectionAttribute = collectionProperty.GetCustomAttributes().OfType<MongoCollectionAttribute>().FirstOrDefault();

            modelBuilder.Entity(entityType, b =>
            {
                b.CollectionName = collectionAttribute?.CollectionName ?? collectionProperty.Name;
            });
        }
        protected virtual void BuildModelFromDbContextInstance(MongoModelBuilder modelBuilder, CafMongoDbContext dbContext)
        {
            dbContext.CreateModel(modelBuilder);
        }
    }
}
