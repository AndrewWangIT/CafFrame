using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //var xxxxxx = new xxx();
            var list1 = new List<string>();
            var list2 = new List<string>();
            for (int i = 0; i < 800000; i++)
            {
                var item1 = Guid.NewGuid().ToString();
                list1.Add(item1);
                if (i%2==0)
                {
                    list2.Add(item1);
                }
                else
                {
                    list2.Add(Guid.NewGuid().ToString());
                }
            }
            var reslut = list1.Intersect(list2);

            Console.ReadKey();
        }
    }
    public class xxqq
    {
        HttpClient httpClient;
        public xxqq()
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
    public class xx
    {
        public xx()
        {
            var a = this.GetType();
        }
        public string aa { get; set; }
    }
    public class xxx : xx
    {
        public string bb { get; set; }
    }
}
