using Microsoft.EntityFrameworkCore;
using ProjectIpInformation.Entities;
using ProjectIpInformation.Repositories.Interfaces;
using ProjectIpInformation.Services.Interfaces;

namespace ProjectIpInformation.Services
{
    public class IpInfoUpdateService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICacheService _cacheService;
        private readonly IIp2CService _iIp2CService;
        private Timer _timer;

        public IpInfoUpdateService(IServiceScopeFactory scopeFactory, ICacheService cacheService, IIp2CService iIp2CService)
        {
            _scopeFactory = scopeFactory;
            _cacheService = cacheService;
            _iIp2CService = iIp2CService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateIpInfo, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void UpdateIpInfo(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var ipRepository = scope.ServiceProvider.GetRequiredService<IIpRepository>();

                var ipList = await ipRepository.GetIpBatchAsync(100);
                var idsCountry = await ipRepository.GetContryBatchAsync();

                foreach (var ip in ipList)
                {
                    try
                    {
                        var ipInfoUpdated = await _iIp2CService.GetIpInfoFromIp2CAsync(ip.IP);

                        if (ipInfoUpdated != null && (ipInfoUpdated.TwoLetterCode != ip.Country.TwoLetterCode))
                        {
                            var countryId = idsCountry.FirstOrDefault(c => c.TwoLetterCode == ipInfoUpdated.TwoLetterCode)?.Id;

                            if (countryId == null)
                            {
                                var newCountry = new Country
                                {
                                    TwoLetterCode = ipInfoUpdated.TwoLetterCode,
                                    ThreeLetterCode = ipInfoUpdated.ThreeLetterCode,
                                    Name = ipInfoUpdated.CountryName,
                                    CreatedAt = DateTime.UtcNow

                                };

                                await ipRepository.AddCountryAsync(newCountry);
                                countryId = newCountry.Id;
                                idsCountry.Add(newCountry);
                            }

                            
                            ip.CountryId = countryId.Value;
                            ip.UpdatedAt = DateTime.UtcNow;
                            await ipRepository.UpdateIpAsync(ip);

                            _cacheService.InvalidateIpCache(ip.IP);
                            await _cacheService.SetCachedIpInfoAsync(ip.IP, ipInfoUpdated);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao atualizar IP {ip.IP}: {ex.Message}");
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
