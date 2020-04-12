using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Caf.Grpc.Client.Utility;
using CafApi.GrpcService;
using Capgemini.Frame.Grpc.Server.DynamicGenerator;
using MagicOnion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CafApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        private readonly ITestNewService  _testNewService;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ITestNewService testNewService)
        {
            _testNewService = testNewService;
        }

        [HttpGet]
        public async Task<TestData> GetAsync(string name)
        {
            //var grpcserver = _grpcConnectionUtility.GetRemoteServiceForDirectConnection<ITestService>("TestServiceName");
            //var data = await grpcserver.GetTestData();
            var data = await _testNewService.SayHello(name);
            return data;

        }
    }
}
