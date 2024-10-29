using ProjectIpInformation.Dtos;
using ProjectIpInformation.Entities;


namespace ProjectIpInformation.Repositories.Interfaces
{
    public interface IIpRepository
    {
        Task<IpInfoDto> GetIpInfoAsync(string ip);
        Task SaveIpInfoAsync(IpInfoDto ipInfo);
        Task<IEnumerable<IPAddress>> GetAllIps(int batchSize); 
        Task UpdateIpInfoAsync(int ipId, IpInfoDto latestInfo);
        Task<List<IpReportDto>> GetIpReportAsync(string[] countryCodes);
        Task<List<IPAddress>> GetIpBatchAsync(int batchSize);
        Task UpdateIpAsync(IPAddress ipAddress);
        Task<List<Country>> GetContryBatchAsync();
        Task AddCountryAsync(Country country);

    }
}
