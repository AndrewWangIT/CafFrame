using Caf.Grpc.Client.Utility;
using Caf.Grpc.DynamicGenerator;
using Cafgemini.Frame.Grpc.Server.DynamicGenerator;
using MagicOnion;
using MagicOnion.Server;
using MessagePack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CafApi.GrpcService
{
    //public interface ITestNewService
    //{
    //    Task<List<string>> SayHello(string name);
    //}

    //public class TestNewService : ITestNewService
    //{
    //    public Task<List<string>> SayHello(string name)
    //    {
    //        //return Task.FromResult("Hi" + name);
    //        //return Task.FromResult(new xx { aa="Hi" + name });
    //        return Task.FromResult(new List<string> { name });
    //    }
    //}
    [MagicOnionGrpc]
    public interface ITestNewService
    {
        Task<TestData> SayHello(string name);
    }

    public class TestNewService : ITestNewService
    {
        public Task<TestData> SayHello(string name)
        {
            TestData testData = new TestData();
            testData.aa= "Hi" + name;
            testData.bb = 44142;
            for (int i = 0; i < 100; i++)
            {
                testData.cc.Add("dadada");

            }
            return Task.FromResult(testData);
        }
    }

    //public class TestNewServiceNew : ITestService
    //{
    //    private readonly IGrpcConnectionUtility _grpcConnectionUtility;
    //    public TestNewServiceNew(IGrpcConnectionUtility grpcConnectionUtility)
    //    {
    //        _grpcConnectionUtility = grpcConnectionUtility;
    //    }
    //    public async Task<string> GetTestData(string name)
    //    {
    //        var grpcserver =  _grpcConnectionUtility.GetRemoteServiceForDirectConnection<ITestService>("TestServiceName");
    //        var data = await grpcserver.GetTestData(name);
    //        return data;
    //    }
    //}




    [MessagePackObject]
    public class TestData
    {
        [Key(0)]
        public string aa = "";
        [Key(1)]
        public long bb = 1;
        [Key(2)]
        public List<string> cc = new List<string> { };
    }
}
