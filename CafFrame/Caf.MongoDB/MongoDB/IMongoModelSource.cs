namespace Caf.MongoDB.MongoDB
{
    public interface IMongoModelSource
    {
        MongoDbContextModel GetModel(CafMongoDbContext dbContext);
    }
}