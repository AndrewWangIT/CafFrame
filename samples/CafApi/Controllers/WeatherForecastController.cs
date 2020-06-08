using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Caf.Domain.IntegrationEvent;
using Caf.Grpc.Client.Utility;
using Caf.MongoDB.Repository;
using CafApi.Event;
using CafApi.GrpcService;
using Capgemini.Frame.Grpc.Server.DynamicGenerator;
using MagicOnion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongodbTest;

namespace CafApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        private readonly IIntegrationEventBus _integrationEventBus;
        private readonly ITestNewService  _testNewService;
        private readonly IMongoDbRepository<ContentLibraryDbContext, UserBehavior> _mongoDbRepository;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IIntegrationEventBus integrationEventBus, ITestNewService testNewService, IMongoDbRepository<ContentLibraryDbContext, UserBehavior> mongoDbRepository)
        {
            _testNewService = testNewService;
            _mongoDbRepository = mongoDbRepository;
            _integrationEventBus = integrationEventBus;
        }

        [HttpGet]
        public async Task<TestData> GetAsync(string name)
        {
            var a = _mongoDbRepository.QueryList(o=>o.UserId == "123456").ToList();
            await _mongoDbRepository.InsertAsync(new UserBehavior { Channel = "", ContentTag = new List<string> { "tag1", "tag2" }, Type = 1, UserId = "123456" });
            //var grpcserver = _grpcConnectionUtility.GetRemoteServiceForDirectConnection<ITestService>("TestServiceName");
            //var data = await grpcserver.GetTestData();
            var data = await _testNewService.SayHello(name);
            return data;
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
    }
}
