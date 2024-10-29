using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIpInformation.Dtos;
using ProjectIpInformation.Entities;
using ProjectIpInformation.Repositories.Interfaces;

namespace ProjectIpInformation.Repositories
{
    public class IpRepository : IIpRepository
    {
        private readonly ApplicationDbContext _context;

        public IpRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IPAddress>> GetAllIps(int batchSize)
        {
            return await _context.IPAddresses.Take(batchSize).ToListAsync();
        }
       
        public async Task<IpInfoDto> GetIpInfoAsync(string ip)
        {
            var ipEntity = await _context.IPAddresses.Include(c => c.Country)
                .FirstOrDefaultAsync(i => i.IP == ip);

            if (ipEntity == null) return null;

            return new IpInfoDto
            {
                Ip = ipEntity.IP,
                CountryName = ipEntity.Country.Name,
                TwoLetterCode = ipEntity.Country.TwoLetterCode,
                ThreeLetterCode = ipEntity.Country.ThreeLetterCode
            };
        }

        public async Task SaveIpInfoAsync(IpInfoDto ipInfo)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.TwoLetterCode == ipInfo.TwoLetterCode);

            if (country == null)
            {
                country = new Country
                {
                    Name = ipInfo.CountryName,
                    TwoLetterCode = ipInfo.TwoLetterCode,
                    ThreeLetterCode = ipInfo.ThreeLetterCode,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Countries.Add(country);
            }

            var ipAddress = new IPAddress
            {
                IP = ipInfo.Ip,
                CountryId = country.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.IPAddresses.Add(ipAddress);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateIpInfoAsync(int ipId, IpInfoDto latestInfo)
        {
            var ipAddress = await _context.IPAddresses.FindAsync(ipId);
            if (ipAddress != null)
            {
                ipAddress.Country.Name = latestInfo.CountryName;
                ipAddress.Country.TwoLetterCode = latestInfo.TwoLetterCode;
                ipAddress.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<IpReportDto>> GetIpReportAsync(string[] countryCodes)
        {
            var query = @"
                SELECT c.Name AS CountryName,
                       COUNT(i.Id) AS AddressesCount,
                       MAX(i.UpdatedAt) AS LastAddressUpdated
                FROM Countries c
                LEFT JOIN IPAddresses i ON c.Id = i.CountryId
                WHERE (@CountryCodes IS NULL OR c.TwoLetterCode IN @CountryCodes)
                GROUP BY c.Name";

            var parameters = new { CountryCodes = countryCodes?.Any() == true ? countryCodes : null };

            using (var connection = _context.Database.GetDbConnection())
            {
                return (await connection.QueryAsync<IpReportDto>(query, parameters)).ToList();
            }
        }

        public async Task<List<IPAddress>> GetIpBatchAsync(int batchSize)
        {
            return await _context.IPAddresses
                .Include(ip => ip.Country)
                .OrderBy(ip => ip.UpdatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<Country>> GetContryBatchAsync()
        {
            return await _context.Countries
                .ToListAsync();
        }



        public async Task UpdateIpAsync(IPAddress ipAddress)
        {
            _context.IPAddresses.Update(ipAddress);
            await _context.SaveChangesAsync();
        }

        public Task AddCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            return _context.SaveChangesAsync();
        }
    }   
}
