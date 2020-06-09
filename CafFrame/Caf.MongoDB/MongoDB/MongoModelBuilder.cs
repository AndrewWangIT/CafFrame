using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoModelBuilder
    {
        private readonly Dictionary<Type, MongoEntityModelBuilder> _entityModelBuilders=new Dictionary<Type, MongoEntityModelBuilder>();
        private static readonly object SyncObj = new object();
        public MongoDbContextModel Build()
        {
            lock (SyncObj)
            {
                var entityModels = _entityModelBuilders
                    .Select(x => x.Value)
                    .Cast<IMongoEntityModel>()
                    .ToImmutableDictionary(x => x.EntityType, x => x);

                var baseClasses = new List<Type>();

                foreach (var entityModel in entityModels.Values)
                {
                    var map = entityModel.As<IHasBsonClassMap>().GetMap();
                    if (!BsonClassMap.IsClassMapRegistered(map.ClassType))
                    {
                        BsonClassMap.RegisterClassMap(map);
                    }

                    //baseClasses.AddRange(entityModel.EntityType.GetBaseClasses(includeObject: false));
                }

                //baseClasses = baseClasses.Distinct().ToList();

                //foreach (var baseClass in baseClasses)
                //{
                //    if (!BsonClassMap.IsClassMapRegistered(baseClass))
                //    {
                //        var map = new BsonClassMap(baseClass);
                //        map.AutoMap();
                //        BsonClassMap.RegisterClassMap(map);
                //    }
                //}

                return new MongoDbContextModel(entityModels);
            }
        }
        public virtual void Entity(Type entityType, Action<MongoEntityModelBuilder> buildAction = null)
        {

            var model = _entityModelBuilders.GetOrAdd(
                entityType,
                () => GetDefaultEntityModel(entityType)
                );

            buildAction?.Invoke(model);
        }
        private MongoEntityModelBuilder GetDefaultEntityModel(Type entityType)
        {
            MongoEntityModelBuilder builder = new MongoEntityModelBuilder();
            builder.EntityType = entityType;
            var _bsonClassMap = new BsonClassMap(entityType);
            _bsonClassMap.AutoMap();
            builder.BsonMap = _bsonClassMap;
            return builder;
        }
    }
    public class MongoEntityModelBuilder: IMongoEntityModel, IHasBsonClassMap
    {
        public Type EntityType { get; set; }

        public string CollectionName { get; set; }

        public BsonClassMap BsonMap { get; set; }

        public BsonClassMap GetMap()
        {
            return BsonMap;
        }
    }
    public interface IHasBsonClassMap
    {
        BsonClassMap GetMap();
    }
}
