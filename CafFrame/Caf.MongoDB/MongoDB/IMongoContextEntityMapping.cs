using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface IMongoContextEntityMapping
    {
        Type GetDbContextByEntity<TEntity>();
        void LoadMaping(Type dbContextType);
    }
}
