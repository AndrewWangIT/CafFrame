using Caf.DynamicWebApi;
using Caf.DynamicWebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CafApi.DynamicServices
{
    [DynamicWebApi]
    public class TestAppService:IDynamicWebApiService
    {
        public string Get(int id)
        {
            return "";
        }



        public string TestDynamicApi(int id)
        {
            return "";
        }
    }
}