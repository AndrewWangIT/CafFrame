using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<xx>();
            //HttpClient httpClient = new HttpClient();
           
        }
    }
    public class xx
    {
        HttpClient httpClient;
        public xx()
        {
            httpClient = new HttpClient();
           
        }

        [Benchmark]
        public async Task TestWebApi()
        {
            var a = await httpClient.GetAsync("http://localhost:5000/WeatherForecast/WebApi");
            a.EnsureSuccessStatusCode();
        }
        [Benchmark]
        public async Task TestGRPC()
        {
            var a = await httpClient.GetAsync("http://localhost:5000/WeatherForecast/GRPC");
            a.EnsureSuccessStatusCode();
        }

    }
}
