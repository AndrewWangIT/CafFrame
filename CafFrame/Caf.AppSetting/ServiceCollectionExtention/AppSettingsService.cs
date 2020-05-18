using Caf.AppSetting.DbContextService;
using Caf.AppSetting.Model;
using Caf.Cache;
using Caf.Core.DataModel.Http;
using Caf.Core.Utils.Ext;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caf.AppSetting.ServiceCollectionExtention
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly CafAppsettingDbContext _dbContext;
        private readonly ICafCache _cafCache;

        public AppSettingsService(CafAppsettingDbContext dbContext, ICafCache cafCache)  
        {
            _dbContext = dbContext;
            _cafCache = cafCache;
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
                if (ret == null)
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
            }

            return ret;
        }
    }
}
