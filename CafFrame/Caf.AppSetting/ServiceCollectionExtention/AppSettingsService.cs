using Caf.AppSetting.DbContextService;
using Caf.Cache;
using Caf.Core.AppSetting;
using Caf.Core.DataModel.Http;
using Caf.Core.Utils.Ext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Caf.AppSetting.ServiceCollectionExtention
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly CafAppsettingDbContext _dbContext;
        private readonly ICafCache _cafCache;
        private readonly AccountOption _accOption;

        private const string backadmin = nameof(backadmin);


        public AppSettingsService(CafAppsettingDbContext dbContext, ICafCache cafCache, IOptionsSnapshot<AccountOption> options)  
        {
            _dbContext = dbContext;
            _cafCache = cafCache;
            _accOption = options.Value;
        }

        public async Task<ResponseBase> DeleteAsync(string key)
        {
            var ret = new ResponseBase();
            var ent = await _dbContext.AppSettingByCafs.FirstOrDefaultAsync(p => p.Key == key);

            if (ent != null)
            {
                _dbContext.AppSettingByCafs.Remove(ent);
                _dbContext.SaveChanges();
                ret.IsSuccess = true;
            }
            else
            {
                ret.Message = $"配置{key}不存在";
            }

            return ret;
        }

        public async Task<string> Get(string key)
        {
            return await Get<string>(key);
        }

        public async Task<T> Get<T>(string key)
        {
            try
            {
                var ret = _cafCache.Get<T>($"{Keys.CafCache}_{key}");
                if (ret == null || !_cafCache.Exists($"{Keys.CafCache}_{key}"))
                {
                    T value = default(T);

                    var entity = await _dbContext.AppSettingByCafs.FirstOrDefaultAsync(p => p.Key == key);
                    string tempValue = entity?.Value;
                    if (!string.IsNullOrEmpty(tempValue))
                    {
                        if (typeof(T).IsPrimitive)
                        {
                            value = JsonConvert.DeserializeObject<T>(tempValue);
                        }
                        else
                        {
                            var type = typeof(T);
                            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                if (tempValue != null)
                                {
                                    NullableConverter converter = new NullableConverter(type);
                                    type = converter.UnderlyingType;
                                }
                            }
                            value = (T)Convert.ChangeType(tempValue, type);
                        }

                        ret = value;
                        _cafCache.Put<T>($"{Keys.CafCache}_{key}", ret, 3600);
                    }
                }

                if (ret == null)
                {
                    throw new Exception($"未配置key={key}的项");
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<List<AppSettingViewModel>> GetListAsync(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponse<AppSettingViewModel>> QueryPageList(AppSettingPageRequest request)
        {
            PagedResponse<AppSettingViewModel> result = new PagedResponse<AppSettingViewModel>();
            try
            {
                Expression<Func<AppSettingViewModel, bool>> wherepre = a => !a.IsDeleted;// a.Key.Contains(request.Key);
                if (!string.IsNullOrWhiteSpace(request.Key)) 
                {
                    Expression<Func<AppSettingViewModel, bool>> wherenext = w=> w.Key.Contains(request.Key.Trim());
                    wherepre = wherepre.AndAlso(wherenext);
                }
                result.IsSuccess = true;
                result.Total = await _dbContext.AppSettingByCafs.AsNoTracking().CountAsync(wherepre);
                result.Datas = await _dbContext.AppSettingByCafs.AsNoTracking().Where(wherepre).Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResponseBase> UpdateOrCreateAsync(AppSettingViewModel model)
        {
            ResponseBase ret = new ResponseBase();
            var entity = _dbContext.AppSettingByCafs.FirstOrDefault(p => p.Key == model.Key);
            if (model.Id==0 && entity!=null)
            {
                ret.IsSuccess = false;
                ret.Message = "新增key重复！";
                return ret;
            }
            if (entity != null)
            {
                entity.LatestModifiedTime = DateTime.Now;
                entity.Value = model.Value;
                entity.Description = model.Description;
                _dbContext.AppSettingByCafs.Update(entity);
            }
            else
            {
                model.CreateTime = DateTime.Now;
                await _dbContext.AppSettingByCafs.AddAsync(model);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                ret.IsSuccess = true;
                _cafCache.Remove($"{Keys.CafCache}_{model.Key}");//清空缓存
                //_cafCache.Put<string>($"{Keys.CafCache}_{model.Key}", model.Value, 3600);
            }

            return ret;
        }

        public async Task<ResponseBase<LoginResViewModel>> LoginAsync(LoginViewModel model) 
        {
            ResponseBase<LoginResViewModel> ret = new ResponseBase<LoginResViewModel>() { IsSuccess=true };
            if (_accOption == null)
            {
                ret.IsSuccess = false;
                ret.Message = "用户或密码未正确配置！";
                return ret;
            }
            //await _dbContext.AppSettingByCafs.FirstOrDefaultAsync(u => u.Key == model.UserName && u.Value==model.Password);
            if (_accOption.UserName!=model.UserName || _accOption.Password!=model.Password)  
            {
                ret.IsSuccess = false;
                ret.Message = "用户或密码不正确！";
                return ret;
            }
            var md5 = MD5Encrypt($"Caf:{model.UserName}:{model.Password}");
            _cafCache.Put(backadmin, md5, 60 * 20);
            ret.Data = new LoginResViewModel { Token = md5 };
            return ret;
        }

        public async Task<ResponseBase> IsLogin(string token) 
        {
            ResponseBase ret = new ResponseBase() { IsSuccess = true };
            var enti = _cafCache.Get(backadmin);
            if (enti == null || enti!=token)   
            {
                ret.IsSuccess = false;
                ret.Message = "重新登录";
                return ret;
            }
            var md5 = MD5Encrypt($"Caf:{_accOption.UserName}:{_accOption.Password}");
            if (token!=md5)
            {
                ret.IsSuccess = false;
                ret.Message = "密码已变更，重新登录";
                return ret;
            }
            return ret;
        }

        private static string MD5Encrypt(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "");
        }
    }
}
