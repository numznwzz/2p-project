using System.Security.Cryptography;
using System.Text;
using Domain.Environments;
using Domain.Repository;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Repository
{
    public class MemoryStore : IMemoryStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEnvironmentsConfig _env;

        public MemoryStore(IDistributedCache distributedCache, IEnvironmentsConfig env)
        {
            _env = env;
            _distributedCache = distributedCache;
        }

        public async Task SetAsync(string cacheKey, Object value, double expireTime = 1, string path = "default")
        {
            //cacheKey = await ConvertMD5(cacheKey);
            var date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            cacheKey = date + ":" + path + ":" + cacheKey;
            var options =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expireTime));

            string val = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            await _distributedCache.SetStringAsync(cacheKey, val, options);
        }

        public async Task<string> GetAsync(string cacheKey, string path = "default")
        {
           // cacheKey = await ConvertMD5(cacheKey);
            var date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            cacheKey = date + ":" + path + ":" + cacheKey;
            var value = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(value)) return value;

            return null;
        }
        
        public async Task SetNoDateAsync(string cacheKey, Object value, double expireTime = 1, string path = "default")
        {
            //cacheKey = await ConvertMD5(cacheKey);
            cacheKey = path + ":" + cacheKey;
            var options =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expireTime));

            string val = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            await _distributedCache.SetStringAsync(cacheKey, val, options);
        }

        public async Task<string> GetNoDateAsync(string cacheKey, string path = "default")
        {
            cacheKey = path + ":" + cacheKey;
            var value = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(value)) return value;

            return null;
        }

        public async Task<string> MakeKeyAsync(int val1, int val2, int val3)
        {
            return $"{val1}_{val2}_{val3}";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private async Task<string> ConvertMD5(string key)
        {
            using (var hash = MD5.Create())
            {
                var result = string.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(key)
                    )
                    select ba.ToString("x2")
                );

                return result;
            }
        }
    }
}