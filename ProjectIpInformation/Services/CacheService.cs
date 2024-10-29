using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ProjectIpInformation.Dtos;
using ProjectIpInformation.Services.Interfaces;

namespace ProjectIpInformation.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<IpInfoDto> GetCachedIpInfoAsync(string ip)
        {
            var cacheKey = $"IPInfo_{ip}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                return JsonConvert.DeserializeObject<IpInfoDto>(cachedData);
            }
            return null;
        }

        public async Task SetCachedIpInfoAsync(string ip, IpInfoDto ipInfo)
        {
            var cacheKey = $"IPInfo_{ip}";
            var cacheData = JsonConvert.SerializeObject(ipInfo);
            await _cache.SetStringAsync(cacheKey, cacheData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });
        }

        public void InvalidateIpCache(string ip)
        {
            _cache.Remove(ip); 
        }
    }
}
