using ProjectIpInformation.Dtos;
using ProjectIpInformation.Services.Interfaces;

namespace ProjectIpInformation.Services
{
    public class Ip2CService : IIp2CService
    {
        private readonly HttpClient _httpClient;

        public Ip2CService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IpInfoDto> GetIpInfoFromIp2CAsync(string ip)
        {
            var response = await _httpClient.GetAsync($"http://ip2c.org/{ip}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ipInfo = ParseIp2CResponse(content);
                ipInfo.Ip = ip;

                if (ipInfo != null)
                    return ipInfo;
            }

            return null;
        }

        private IpInfoDto ParseIp2CResponse(string response)
        {
            var parts = response.Split(';');
            if (parts.Length >= 3 && parts[0] == "1") 
            {
                return new IpInfoDto
                {
                    TwoLetterCode = parts[1],
                    ThreeLetterCode = parts[2],
                    CountryName = parts.Length > 3 ? parts[3] : ""
                };
            }

            return null;
        }
    }
}
