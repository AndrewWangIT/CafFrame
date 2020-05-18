using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caf.AppSetting.Model;
using Caf.Core.DataModel.Http;

namespace Caf.AppSetting.ServiceCollectionExtention
{
    public interface IAppSettingsService
    {
        Task<ResponseBase> DeleteAsync(string key);

        Task<ResponseBase> UpdateOrCreateAsync(AppSettingViewModel model);

        Task<string> Get(string key);

        Task<T> Get<T>(string key);

        Task<PagedResponse<AppSettingViewModel>> QueryPageList(AppSettingPageRequest request);

        Task<List<AppSettingViewModel>> GetListAsync(params string[] keys);
    }
}
