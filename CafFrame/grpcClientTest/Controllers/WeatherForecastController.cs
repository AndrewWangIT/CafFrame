using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace grpcClientTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ITestNewService _testNewService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient httpClient;
        public WeatherForecastController( ITestNewService testNewService,IHttpClientFactory httpClientFactory)
        {
            _testNewService = testNewService;
            _clientFactory = httpClientFactory;
            httpClient = _clientFactory.CreateClient("webapi");
        }

        [HttpGet]
        public async Task<string> Get()
        {
            Stopwatch stopwatch = new Stopwatch();
           
            stopwatch.Start();
            for (var i=0;i < 20000;i++)
            {
                var response = await httpClient.GetAsync("/weatherforecast?name=hahahaha");
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStringAsync();
                var a = JsonConvert.DeserializeObject<TestData>(responseStream);
            }
            stopwatch.Stop();
            var time1 = stopwatch.ElapsedMilliseconds;
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (var i = 0; i < 20000; i++)
            {
                var b = await _testNewService.SayHello("hahahaha");
            }
            stopwatch.Stop();
            var time2 = stopwatch1.ElapsedMilliseconds;
            return $"{time1}-{time2}";
        }

        [HttpGet("WebApi")]
        public async Task<string> GetWebApi()
        {
            for (var i = 0; i < 100; i++)
            {
                var response = await httpClient.GetAsync("/weatherforecast?name=hahahaha");
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStringAsync();
                var a = JsonConvert.DeserializeObject<TestData>(responseStream);
            }
            return "ok";

        }

        [HttpGet("GRPC")]
        public async Task<string> GetGRPC()
        {
            for (var i = 0; i < 100; i++)
            {
                var b = await _testNewService.SayHello("hahahaha");
            }
            return "ok";
        }
    }
}
