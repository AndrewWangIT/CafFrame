using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caf.Domain.IntegrationEvent;
using Caf.Grpc.Client.Utility;
using Caf.MongoDB.Repository;
using CafApi.Event;
using CafApi.GrpcService;
using Cafgemini.Frame.Grpc.Server.DynamicGenerator;
using MagicOnion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongodbTest;

namespace CafApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        private readonly IIntegrationEventBus _integrationEventBus;
        private readonly ITestNewService  _testNewService;
        private readonly IMongoDbRepository<UserBehavior> _mongoDbRepository;
        private readonly IMongoDbRepository<UserBehaviorGroup> _userBehaviorGroupRepository;
        private readonly IOptionsSnapshot<AccountOptions> _accountOptions;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IOptionsSnapshot<AccountOptions> accountOptions ,IMongoDbRepository<UserBehaviorGroup> userBehaviorGroupRepository,IIntegrationEventBus integrationEventBus, ITestNewService testNewService, IMongoDbRepository<UserBehavior> mongoDbRepository)
        {
            _accountOptions= accountOptions;
            _testNewService = testNewService;
            _mongoDbRepository = mongoDbRepository;
            _integrationEventBus = integrationEventBus;
            _userBehaviorGroupRepository = userBehaviorGroupRepository;
        }

        [HttpGet]
        public async Task<int> GetAsync(string name)
        {
            var data1 = await _testNewService.SayHello(name);
            var a = _accountOptions.Value.ResetPasswordCodeExpire;
            //throw new Exception("s");
            //var a = _mongoDbRepository.QueryList(o=>o.UserId == "123456").ToList();
            //var aa = MockuserBehavior();
            //await _mongoDbRepository.InsertAsync(aa);
            //await _mongoDbRepository.InsertAsync(new UserBehavior { Channel = "", ContentTag = new List<string> { "tag1", "tag2" }, Type = 1, UserId = "123456" });
            //var grpcserver = _grpcConnectionUtility.GetRemoteServiceForDirectConnection<ITestService>("TestServiceName");
            //var data = await grpcserver.GetTestData();
            var data = await _testNewService.SayHello(name);
            return a;
        }
        [HttpGet("mockData")]
        public async Task<string> mockData()
        {
            _userBehaviorGroupRepository.Database.DropCollection(_mongoDbRepository.CollectionName);
            _mongoDbRepository.Database.DropCollection(_mongoDbRepository.CollectionName);   
            for (int ii = 0; ii < 500; ii++)
            {
                List<UserBehavior> userBehaviors = new List<UserBehavior>();
                for (int i = 0; i < 10000; i++)
                {
                    var aa = MockuserBehavior();
                    userBehaviors.Add(aa);
                }
                await _mongoDbRepository.Collection.InsertManyAsync(userBehaviors);
                

            }

            return "OK";
        }
        [HttpGet("mockData1111")]
        public async Task<string> mockData11()
        {
            var a = _userBehaviorGroupRepository.Collection.AsQueryable().Where(o => o.UserId == "76827").ToList();
            return "";
        }
        [HttpGet("mockData1")]
        public async Task<string> mockData1()
        {       
            
            List<Behavior> Behaviors = new List<Behavior>();
            var q = MockuserBehavior();
            for (int i = 0; i < 100; i++)
            {
                //1.阅读 2.点赞 3.转发
                var aa = MockuserBehavior();
                if(aa.Type==1)
                {
                    Behaviors.Add(new Behavior {  ViewSeek=1, ReadTime=20, ActionTime = DateTime.Now, Tags = aa.ContentTag, Type = aa.Type,ContentId= aa.ContentId ,Channel=aa.Channel});
                }
                else if(aa.Type == 2)
                {
                    Behaviors.Add(new Behavior { ReadTime=0, ForwardSeek=1,ActionTime = DateTime.Now, Tags = aa.ContentTag, Type = aa.Type, ContentId = aa.ContentId, Channel = aa.Channel });
                }
                else if (aa.Type == 3)
                {
                    Behaviors.Add(new Behavior { ReadTime = 0, LikeSeek = 1,ActionTime = DateTime.Now, Tags = aa.ContentTag, Type = aa.Type, ContentId = aa.ContentId, Channel = aa.Channel });
                }

            }
            var update = new UpdateDefinitionBuilder<UserBehaviorGroup>().PushEach<Behavior>(x => x.Behaviors, Behaviors.Take(20));
            update = update.Set(o => o.LastModificationTime, DateTime.Now);
            var options = new FindOneAndUpdateOptions<UserBehaviorGroup>() { IsUpsert = true, ReturnDocument= ReturnDocument.After };
            var data = await _userBehaviorGroupRepository.Collection.FindOneAndUpdateAsync<UserBehaviorGroup>(o => o.UserId == q.UserId && o.Year== q.Year && o.Month== q.Month && o.Date==q.Date, update, options);
            return "OK";
        }
        [HttpGet("Aggregate")]
        public async Task<string> Aggregate(string name, int age)
        {
            AggregateUnwindOptions<UserBehavior> options = new AggregateUnwindOptions<UserBehavior>();
           var aaa =  new BsonDocument[]
{
    new BsonDocument("$group",
    new BsonDocument
        {
            { "_id",
    new BsonDocument("userId", "$UserId") },
            { "Behaviors",
    new BsonDocument("$push", "$Behaviors") }
        }),
    new BsonDocument("$unwind",
    new BsonDocument("path", "$Behaviors")),
    new BsonDocument("$unwind",
    new BsonDocument("path", "$Behaviors")),
    new BsonDocument("$unwind",
    new BsonDocument("path", "$Behaviors.Tags")),
    new BsonDocument("$group",
    new BsonDocument
        {
            { "_id",
    new BsonDocument
            {
                { "userid", "$_id.userId" },
                { "Tag", "$Behaviors.Tags" }
            } },
            { "dianzan",
    new BsonDocument("$sum", "$Behaviors.Seek") },
            { "dianzan1",
    new BsonDocument("$sum", "$Behaviors.Seek") },
            { "dianzan2",
    new BsonDocument("$sum", "$Behaviors.Seek") },
            { "dianzan3",
    new BsonDocument("$sum", "$Behaviors.Seek") }
        }),
    new BsonDocument("$group",
    new BsonDocument
        {
            { "_id",
    new BsonDocument("userid", "$_id.userid") },
            { "summer",
    new BsonDocument("$push",
    new BsonDocument
                {
                    { "Tag", "$_id.Tag" },
                    { "dianzan", "$dianzan" }
                }) }
        }) };

            var pipelinedef =PipelineDefinition<UserBehaviorGroup, xxx>.Create(aaa);
            var pipeline = _userBehaviorGroupRepository.Collection.Aggregate(pipelinedef, new AggregateOptions { AllowDiskUse = true });
                



                //.Unwind<UserBehavior, UserBehaviorTemp>(o => o.ContentTag).Group(o => new { Channel = o.Channel, Type = o.Type, ContentTag = o.ContentTag, UserId = o.UserId }, g => new { key = g.Key, Count = g.Count() })
                //.Project(x => new xx { Channel = x.key.Channel, Type = x.key.Type, ContentTag = x.key.ContentTag, UserId = x.key.UserId, Count = x.Count })
                //.Limit(10000).Out("test1");
            var result = await pipeline.ToListAsync();
            return "OK";
        }
        
        [HttpGet("publish")]
        public async Task<string> publishMessage(string name,int age)
        {
            MyIntegrationData myIntegrationData = new MyIntegrationData();
            myIntegrationData.Name = name;
            myIntegrationData.Age = age;
            myIntegrationData.Key = name;
            await _integrationEventBus.publish(myIntegrationData);
            return "OK";
        }


        private UserBehavior MockuserBehavior()
        {
            UserBehavior behavior = new UserBehavior();
            Random random = new Random();
            var index = random.Next(0, 9);
            behavior.Channel = Channels[index];
            behavior.Type = random.Next(1, 4);
            behavior.UserId = random.Next(1, 100889).ToString();
            behavior.ContentId = random.Next(1, 100889).ToString();
            behavior.ContentTag = new List<string>();
            behavior.Date = dates[index].ToString("yyyyMMdd");
            behavior.Year = dates[index].Year;
            behavior.Month = dates[index].Month;
            for (int i = 0; i < random.Next(1, 6); i++)
            {
                behavior.ContentTag.Add(Tags[random.Next(0, 9)]);
            }
            return behavior;
        }
        private List<DateTime> dates = new List<DateTime>() {DateTime.Now, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-6), DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-4) };
        private List<string> Channels = new List<string> { "Channel1", "Channel2", "Channel3", "Channel4", "Channel5", "Channel6", "Channel7", "Channel8", "Channel9", "Channel10" };
        private List<string> Tags = new List<string> { "Tag1", "Tag2", "Tag3", "Tag4", "Tag5", "Tag6", "Tag7", "Tag8", "Tag9", "Tag10" };
    }
    //Channel = x.key.Channel, Type = x.key.Type, ContentTag = x.key.ContentTag, UserId = x.key.UserId, Count = x.Count 
    public class xx
    {
        public ObjectId Id { get; set; }
        public string Channel { get; set; }
        public int Type { get; set; }
        public string ContentTag { get; set; }
        public string UserId { get; set; }
        public int Count { get; set; }
    }

    public class xxx
    {
        public key _id { get; set; }
        public List<summer> summer { get; set; }
    }
    public class key
    {
        public string userid { get; set; }
    }
    public class summer
    {
        public string Tag { get; set; }
        public int dianzan { get; set; }
    }
}
