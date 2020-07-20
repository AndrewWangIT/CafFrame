using Caf.Core.DataModel.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core.AppSetting
{
    public interface IAppSettingsService
    {
        Task<ResponseBase> DeleteAsync(string key);

        Task<ResponseBase> UpdateOrCreateAsync(AppSettingViewModel model);

        Task<string> Get(string key);

        Task<T> Get<T>(string key);

        Task<PagedResponse<AppSettingViewModel>> QueryPageList(AppSettingPageRequest request);

        Task<List<AppSettingViewModel>> GetListAsync(params string[] keys);

        Task<ResponseBase<LoginResViewModel>> LoginAsync(LoginViewModel model);

        Task<ResponseBase> IsLogin(string token);
    }
}
