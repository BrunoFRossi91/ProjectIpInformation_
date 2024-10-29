using ProjectIpInformation.Dtos;

namespace ProjectIpInformation.Services.Interfaces
{
    public interface ICacheService
    {
        Task<IpInfoDto> GetCachedIpInfoAsync(string ip);
        Task SetCachedIpInfoAsync(string ip, IpInfoDto ipInfo);
        void InvalidateIpCache(string ip);
    }
}
