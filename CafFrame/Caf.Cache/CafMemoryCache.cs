using Caf.Core.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Cache
{
    public class CafMemoryCache : ICafCache, ISingleton
    {
        private readonly IMemoryCache _cache;

        public CafMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public string Get(string key)
        {
            return _cache.Get(key)?.ToString();
        }

        public void Put<T>(string key, T value, int expiredSeconds)
        {
            if (expiredSeconds == -1)
            {
                _cache.Set(key, value);
            }
            else
            {
                _cache.Set(key, value, TimeSpan.FromSeconds(expiredSeconds));
            }

        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task<T> TryGetAsync<T>(string key, Func<Task<T>> create, int expiredSeconds)
        {
            if (!_cache.TryGetValue(key, out T value) && create != null)
            {
                value = await create();
                if (value != null && expiredSeconds > 0)
                {
                    Put(key, value, expiredSeconds);
                }
            }
            return value;
        }
    }
}
