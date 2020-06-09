using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface IMongoDbContextProvider<out TMongoDbContext>
        where TMongoDbContext : ICafMongoDbContext
    {
        TMongoDbContext GetDbContext();
    }
}
