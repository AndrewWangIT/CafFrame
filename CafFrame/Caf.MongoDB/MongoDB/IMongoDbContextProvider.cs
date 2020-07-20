using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface IMongoDbContextProvider<TMongoDbContext>
    {
        ICafMongoDbContext GetDbContext();
    }
}
