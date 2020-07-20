using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caf.AppSetting.Model;
using Caf.AppSetting.ServiceCollectionExtention;
using Caf.Core.DataModel.Http;
using Microsoft.AspNetCore.Mvc;

namespace Caf.AppSetting.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AppSettingByCafsController : Controller
    {
        private readonly IAppSettingsService _appSettingsService;
        public AppSettingByCafsController(IAppSettingsService appSettingsService) 
        {
            _appSettingsService = appSettingsService;
        }

        [HttpGet]
        public string Heart()
        {
            return "OK";
        }

        [HttpPost]
        public async Task<ResponseBase<LoginResViewModel>> Login([FromBody]LoginViewModel model) 
        {
            return await _appSettingsService.LoginAsync(model);
        }

        [HttpGet]
        public async Task<ResponseBase> IsLogin(string token) 
        {
            return await _appSettingsService.IsLogin(token);
        }

        [HttpGet]
        public async Task<ResponseBase> Delete(string key) 
        {
            return await _appSettingsService.DeleteAsync(key);
        }

        [HttpPost]
        public async Task<ResponseBase> UpdateOrCreate([FromBody]AppSettingViewModel model) 
        {
            return await _appSettingsService.UpdateOrCreateAsync(model);
        }

        [HttpGet]
        public async Task<string> Get(string key) 
        {
            return await _appSettingsService.Get(key);
        }

        [HttpPost]
        public async Task<PagedResponse<AppSettingViewModel>> QueryPageList([FromBody]AppSettingPageRequest request) 
        {
            return await _appSettingsService.QueryPageList(request);
        }

    }
}