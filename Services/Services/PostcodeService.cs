using AutoMapper;
using Common.Dtos;
using Common.Models;
using Db;
using Db.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Services.Interfaces;

namespace Services.Services
{
    public class PostcodeService : IPostcodeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PostcodeService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task ImportAupFromExcelAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            var existingRegions = await _dbContext.Regions
                .ToDictionaryAsync(r => r.Name, r => r.Code);

            var existingDistricts = await _dbContext.Districts
                .ToDictionaryAsync(d => d.Name, d => d.Code);

            for (int row = 3; row <= rowCount; row++)
            {
                var regionName = worksheet.Cells[row, 2].Text;
                var districtName = worksheet.Cells[row, 4].Text;
                var cityName = worksheet.Cells[row, 5].Text;
                var postcode = worksheet.Cells[row, 6].Text;
                var cityCode = worksheet.Cells[row, 9].Text.Truncate(20);

                if (!existingRegions.ContainsKey(regionName))
                {
                    var newRegion = new Region { Name = regionName };
                    _dbContext.Regions.Add(newRegion);
                    await _dbContext.SaveChangesAsync();
                    existingRegions[regionName] = newRegion.Code;
                }

                if (!existingDistricts.ContainsKey(districtName))
                {
                    var newDistrict = new District { Name = districtName };
                    _dbContext.Districts.Add(newDistrict);
                    await _dbContext.SaveChangesAsync();
                    existingDistricts[districtName] = newDistrict.Code;
                }

                var districtCode = existingDistricts[districtName];
                var regionCode = existingRegions[regionName];

                var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Code == cityCode)
                    ?? new City { Code = cityCode, Name = cityName, DistrictCode = districtCode, RegionCode = regionCode };

                var aup = new Aup
                {
                    Postcode = postcode,
                    CityName = cityName,
                    City = city,
                    CityCode = cityCode,
                    RegionCode = regionCode,
                    RegionName = regionName,
                    DistrictCode = districtCode,
                    DistrictName = districtName
                };

                _dbContext.Aups.Add(aup);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<List<AupDto>> GetAupsAsync(PaginationSettings paginationSettings)
        {
            return _dbContext.Aups
                .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
                .Take(paginationSettings.PageSize)
                .Select(a => _mapper.Map<AupDto>(a))
                .ToListAsync();
        }

        public async Task<byte[]> ExportAupToExcelAsync()
        {
            var aupData = await _dbContext.Aups.ToListAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("AUP");

            worksheet.Cells[1, 1].Value = "Postcode";
            worksheet.Cells[1, 2].Value = "City Code";
            worksheet.Cells[1, 3].Value = "City Name";
            worksheet.Cells[1, 4].Value = "Region Code";
            worksheet.Cells[1, 5].Value = "Region Name";
            worksheet.Cells[1, 6].Value = "District Code";
            worksheet.Cells[1, 7].Value = "District Name";

            int row = 2;
            foreach (var aup in aupData)
            {
                worksheet.Cells[row, 1].Value = aup.Postcode;
                worksheet.Cells[row, 2].Value = aup.CityCode;
                worksheet.Cells[row, 3].Value = aup.CityName;
                worksheet.Cells[row, 4].Value = aup.RegionCode;
                worksheet.Cells[row, 5].Value = aup.RegionName;
                worksheet.Cells[row, 6].Value = aup.DistrictCode;
                worksheet.Cells[row, 7].Value = aup.DistrictName;
                row++;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream.ToArray();
        }

        public Task<List<string>> GetPostcodesWithoutCityAsync()
        {
            return _dbContext.Aups
                .Where(a => string.IsNullOrEmpty(a.CityCode)
                    && string.IsNullOrEmpty(a.CityName))
                .Select(a => a.Postcode)
                .ToListAsync();
        }

        public Task<List<string>> GetPostcodesWithoutDistrictAsync()
        {
            return _dbContext.Aups
                .Where(a => a.DistrictCode == null
                    && string.IsNullOrEmpty(a.DistrictName))
                .Select(a => a.Postcode)
                .ToListAsync();
        }

        public Task<List<string>> GetPostcodesWithoutRegionAsync()
        {
            return _dbContext.Aups
                .Where(a => a.RegionCode == null
                    && string.IsNullOrEmpty(a.RegionName))
                .Select(a => a.Postcode)
                .ToListAsync();
        }
    }
}