using ProjectIpInformation.Dtos;
using ProjectIpInformation.Repositories.Interfaces;
using ProjectIpInformation.Services.Interfaces;

namespace ProjectIpInformation.Services
{
    public class IpInfoService : IIpInfoService
    {
        private readonly ICacheService _cacheService;
        private readonly IIpRepository _ipRepository;
        private readonly IIp2CService _ip2CService;

        public IpInfoService(ICacheService cacheService, IIpRepository ipRepository, IIp2CService ip2CService)
        {
            _cacheService = cacheService;
            _ipRepository = ipRepository;
            _ip2CService = ip2CService;
        }

        public async Task<IpInfoDto> GetIpInfoAsync(string ip)
        {
            var cachedIpInfo = await _cacheService.GetCachedIpInfoAsync(ip);
            if (cachedIpInfo != null)
            {
                return cachedIpInfo;
            }

            var dbIpInfo = await _ipRepository.GetIpInfoAsync(ip);
            if (dbIpInfo != null)
            {
                await _cacheService.SetCachedIpInfoAsync(ip, dbIpInfo);
                return dbIpInfo;
            }

            var ip2cInfo = await _ip2CService.GetIpInfoFromIp2CAsync(ip);
            if (ip2cInfo != null)
            {
                await _ipRepository.SaveIpInfoAsync(ip2cInfo);
                await _cacheService.SetCachedIpInfoAsync(ip, ip2cInfo);
                return ip2cInfo;
            }

            return null;
        }

        public async Task<List<IpReportDto>> GetIpReportAsync(string[] countryCodes)
        {
            return await _ipRepository.GetIpReportAsync(countryCodes);
        }
    }
}
