using ProjectIpInformation.Dtos;

namespace ProjectIpInformation.Services.Interfaces
{
    public interface IIp2CService
    {
        Task<IpInfoDto> GetIpInfoFromIp2CAsync(string ip);
    }
}
