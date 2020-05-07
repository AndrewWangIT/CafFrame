using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<xx>();
            //string a = "大连市马栏黄河路社区卫生服务中心(别名:马栏黄河路社区卫生服务中心(xx))(别名:大连沙河口马栏黄河路社区卫生服务站)";
            //Regex rx = new Regex(@"\(别名:((?!\(别名).)*\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //MatchCollection result  = rx.Matches(a);
            //foreach (Match m in result)
            //{
            //    Console.WriteLine(m.Value.Substring(0,m.Value.Length-1).Replace("(别名:", ""));
            //}

            for (int i = 0; i < 1000; i++)
            {
                Func<string, Task> workItem = async x => { var a = 1; };
                await workItem("ss");
            }

            Console.ReadKey();
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
