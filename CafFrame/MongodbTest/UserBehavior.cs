using Caf.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongodbTest
{
    [BsonIgnoreExtraElements]
    public class UserBehavior: IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string Channel { get; set; }
        public int Type { get; set; }
        public List<string> ContentTag { get; set; }


    }
}
