using Caf.Cache.Models;
using Caf.Core.DependencyInjection;
using CSRedis;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Cache
{
    public class CafRedisCache : ICafCache//, ISingleton
    {
        static CSRedisClient redisManger = null;
        private readonly CacheOptions _options;
        public CafRedisCache(IOptions<CacheOptions> cacheOptions)
        {
            _options = cacheOptions.Value;
            redisManger = new CSRedisClient(_options.Servers);
        }

        protected virtual string FormatKey(string key)
        {
            return "OPCache:" + key;
        }

        static CSRedisClient GetClient()
        {
            return redisManger;
        }

        public void Clear()
        {
            GetClient().Del("*");
        }

        public T Get<T>(string key)
        {
            return GetClient().Get<T>(FormatKey(key));
        }

        public string Get(string key)
        {
            return GetClient().Get(FormatKey(key));
        }

        public void Put<T>(string key, T value, int expiredSeconds)
        {
            GetClient().Set(FormatKey(key), value, expiredSeconds);
        }

        public void Remove(string key)
        {
            GetClient().Del(FormatKey(key));
        }

        public async Task<T> TryGetAsync<T>(string key, Func<Task<T>> create, int expiredSeconds)
        {
            T value = default(T);
            if (!GetClient().Exists(key) && create != null)
            {
                value = await create();
                if (value != null)
                    Put(key, value, expiredSeconds);
            }
            else
            {
                value = Get<T>(key);
            }
            return value;
        }
    }
}
