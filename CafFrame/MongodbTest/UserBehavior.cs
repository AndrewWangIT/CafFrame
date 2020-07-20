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
        public int Year { get; set; }
        public int Month { get; set; }
        public string Date { get; set; }
        public string ContentId { get; set; }
        public List<string> ContentTag { get; set; }


    }
    [BsonIgnoreExtraElements]
    public class UserBehaviorTemp : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string Channel { get; set; }
        public int Type { get; set; }
        public string ContentTag { get; set; }


    }

    [BsonIgnoreExtraElements]
    public class UserBehaviorGroup : IEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public string NewField { get; set; }
        public string UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Date { get; set; }
        public DateTime LastModificationTime { get; set; }
        public List<Behavior> Behaviors { get; set; }
    }

    public class Behavior
    {
        /// <summary>
        /// 1.阅读 2.点赞 3.转发
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 涉及的标签
        /// </summary>
        public List<string> Tags { get; set; }

        public string Channel { get; set; }
        public DateTime ActionTime { get; set; }
        /// <summary>
        /// 浏览时常
        /// </summary>
        public int ReadTime { get; set; }
        public int ViewSeek { get; set; }
        public int LikeSeek { get; set; }
        public int ForwardSeek { get; set; }
        /// <summary>
        /// ContentId
        /// </summary>
        public string ContentId { get; set; }
    }

}
