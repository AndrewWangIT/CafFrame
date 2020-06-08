using AspectCore.Injector;
using Caf.Core;
using Caf.Core.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class CafMongoDbContext : ICafMongoDbContext, ITransient
    {
        public virtual object CurrentCreatorId { get { return null; } }
        private readonly IMongodbClientFactory _mongodbClientFactory;
        private readonly IMongoModelSource _modelSource;
        private readonly MongoDBContextSource _mongoDBContextSource;
        public MongoDBContextOptions mongoDBContextOptions { get; private set; }
        private IMongoDatabase _database;
        public IMongoDatabase Database
        {
            get
            {
                return _database ?? _mongodbClientFactory.
                    GetClient(mongoDBContextOptions.ConnectionString)
                    .GetDatabase(mongoDBContextOptions.DatabaseName);
            }
            set
            {
                _database = value;
            }
        }

        public MongoClient mongoClient => _mongodbClientFactory.GetClient(mongoDBContextOptions.ConnectionString);

        public CafMongoDbContext(IMongoModelSource modelSource, MongoDBContextSource mongoDBContextSource, IMongodbClientFactory mongodbClientFactory)
        {
            _modelSource = modelSource;
            _mongoDBContextSource = mongoDBContextSource;
            mongoDBContextOptions = _mongoDBContextSource.GetDbOption(this.GetType());
            _mongodbClientFactory = mongodbClientFactory;
        }
        public IMongoCollection<T> Collection<T>()
        {
            return Database.GetCollection<T>(GetCollectionName<T>());
        }
        protected virtual string GetCollectionName<T>()
        {
            return GetEntityModel<T>().CollectionName;
        }

        private IMongoEntityModel GetEntityModel<TEntity>()
        {
            var model = _modelSource.GetModel(this).Entities.GetOrDefault(typeof(TEntity));

            if (model == null)
            {
                throw new CafException("Could not find a model for given entity type: " + typeof(TEntity).AssemblyQualifiedName);
            }
            return model;
        }
        protected internal virtual void CreateModel(MongoModelBuilder modelBuilder)
        {

        }

        public virtual void InitializeDatabase(IMongoDatabase database)
        {
            Database = database;
        }
    }
}
