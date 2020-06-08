using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoCollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }

        public MongoCollectionAttribute()
        {

        }

        public MongoCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
