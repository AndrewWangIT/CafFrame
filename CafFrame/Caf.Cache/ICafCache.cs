using System;
using System.Threading.Tasks;

namespace Caf.Cache
{
    public interface ICafCache
    {
        void Put<T>(string key, T value, int expiredSeconds);
        T Get<T>(string key);
        Task<T> TryGetAsync<T>(string key, Func<Task<T>> create, int expiredSeconds);
        void Remove(string key);
        void Clear();
        string Get(string key);

        bool Exists(string key);
    }
}
