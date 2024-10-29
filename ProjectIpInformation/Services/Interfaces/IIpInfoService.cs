using ProjectIpInformation.Dtos;

namespace ProjectIpInformation.Services.Interfaces

{
    public interface IIpInfoService
    {
        Task<IpInfoDto> GetIpInfoAsync(string ip);

        Task<List<IpReportDto>> GetIpReportAsync(string[] countryCodes);
    }
}
