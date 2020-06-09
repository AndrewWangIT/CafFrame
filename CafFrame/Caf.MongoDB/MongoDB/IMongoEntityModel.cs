using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public interface IMongoEntityModel
    {
        Type EntityType { get; }

        string CollectionName { get; }
    }
}
