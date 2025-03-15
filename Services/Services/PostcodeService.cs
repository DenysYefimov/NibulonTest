using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Dtos;
using Common.Models;
using Db;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Services.Interfaces;

namespace Services.Services
{
    public class PostcodeService : IPostcodeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAupImporter _aupImporter;

        public PostcodeService(ApplicationDbContext dbContext, IMapper mapper, IAupImporter aupImporter)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _aupImporter = aupImporter;
        }

        public async Task ImportAupFromExcelAsync(Stream excelData)
        {
            await _aupImporter.ImportDataAsync(excelData);
        }

        public Task<List<AupDto>> GetAupsAsync(PaginationSettings paginationSettings)
        {
            return _dbContext.Aups
                .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
                .Take(paginationSettings.PageSize)
                .ProjectTo<AupDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<byte[]> ExportAupToExcelAsync()
        {
            var aupData = await _dbContext.Aups.ToListAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("AUP");

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Postcode";
            worksheet.Cells[1, 3].Value = "City Code";
            worksheet.Cells[1, 4].Value = "City Name";
            worksheet.Cells[1, 5].Value = "Region Code";
            worksheet.Cells[1, 6].Value = "Region Name";
            worksheet.Cells[1, 7].Value = "District Code";
            worksheet.Cells[1, 8].Value = "District Name";

            int row = 2;
            foreach (var aup in aupData)
            {
                worksheet.Cells[row, 1].Value = aup.Id;
                worksheet.Cells[row, 2].Value = aup.Postcode;
                worksheet.Cells[row, 3].Value = aup.CityCode;
                worksheet.Cells[row, 4].Value = aup.CityName;
                worksheet.Cells[row, 5].Value = aup.RegionCode;
                worksheet.Cells[row, 6].Value = aup.RegionName;
                worksheet.Cells[row, 7].Value = aup.DistrictCode;
                worksheet.Cells[row, 8].Value = aup.DistrictName;
                ++row;
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