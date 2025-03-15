using Db;
using Db.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Services.Interfaces;

namespace Services.Services;

public class AupImporter : IAupImporter
{
    private readonly ApplicationDbContext _context;

    static AupImporter()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public AupImporter(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task ImportDataAsync(Stream excelData)
    {
        await PreFillDictionariesAsync();
        await ProcessExcelDataAsync(excelData);
    }

    private async Task ProcessExcelDataAsync(Stream excelData)
    {
        using var package = new ExcelPackage(excelData);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension.Rows;

        var aupInfos = new List<Aup>(rowCount);
        var cities = new List<City>();

        for (int row = 3; row <= rowCount; row++)
        {
            var regionName = worksheet.Cells[row, 2].Text;
            var districtName = worksheet.Cells[row, 4].Text;
            var cityName = worksheet.Cells[row, 5].Text;
            var postcode = worksheet.Cells[row, 6].Text;
            var cityCode = worksheet.Cells[row, 9].Text.Truncate(20);

            var regionCode = await GetOrCreateNewRegionAsync(regionName);
            var districtCode = await GetOrCreateNewDistrictAsync(districtName);
            var cityByCode = GetOrCreateNewCity(cityCode, cityName, regionCode, districtCode);

            var newAup = new Aup
            {
                Postcode = postcode,
                CityCode = cityByCode.Code,
                CityName = cityName,
                RegionCode = regionCode,
                RegionName = regionName,
                DistrictCode = districtCode,
                DistrictName = districtName
            };

            cities.Add(cityByCode);
            aupInfos.Add(newAup);
        }

        await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Cities.AddRange(cities);
            _context.Aups.AddRange(aupInfos);
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task PreFillDictionariesAsync()
    {
        _regions =
            await _context.Regions
                .ToDictionaryAsync(reg => reg.Name, reg => reg.Code, StringComparer.OrdinalIgnoreCase);

        _districts =
            await _context.Districts
                .ToDictionaryAsync(dist => dist.Name, dist => dist.Code, StringComparer.OrdinalIgnoreCase);

        _cities =
            await _context.Cities
                .ToDictionaryAsync(city => city.Code, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<int> GetOrCreateNewRegionAsync(string regionName)
    {
        if (_regions.TryGetValue(regionName, out var regionCode))
        {
            return regionCode;
        }

        var knownRegion =
            await _context.Regions
                .AsNoTracking()
                .FirstOrDefaultAsync(reg => reg.Name == regionName);
        if (knownRegion != null)
        {
            _regions[knownRegion.Name] = knownRegion.Code;
            return knownRegion.Code;
        }

        var newRegion = new Region { Name = regionName };
        _context.Regions.Add(newRegion);
        await _context.SaveChangesAsync();
        _regions[regionName] = newRegion.Code;
        return newRegion.Code;
    }

    private async Task<int> GetOrCreateNewDistrictAsync(string districtName)
    {
        if (_districts.TryGetValue(districtName, out var districtCode))
        {
            return districtCode;
        }

        var knownDistrict =
            await _context.Districts
                .AsNoTracking()
                .FirstOrDefaultAsync(dist => dist.Name == districtName);
        if (knownDistrict != null)
        {
            _districts[knownDistrict.Name] = knownDistrict.Code;
            return knownDistrict.Code;
        }

        var newDistrict = new District { Name = districtName };
        _context.Districts.Add(newDistrict);
        await _context.SaveChangesAsync();
        _districts[districtName] = newDistrict.Code;
        return newDistrict.Code;
    }

    private City GetOrCreateNewCity(string cityCode, string cityName, int regionCode, int districtCode)
    {
        if (_cities.TryGetValue(cityCode, out var cityInfo))
        {
            return cityInfo;
        }

        var newCity =
            new City { Code = cityCode, Name = cityName, RegionCode = regionCode, DistrictCode = districtCode };
        _cities[cityCode] = newCity;
        return newCity;
    }

    private Dictionary<string, int> _regions = new(1, StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, int> _districts = new(1, StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, City> _cities = new(1, StringComparer.OrdinalIgnoreCase);
}
