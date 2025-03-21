﻿using Common.Dtos;
using Common.Models;

namespace Services.Interfaces
{
    public interface IPostcodeService
    {
        Task<List<AupDto>> GetAupsAsync(PaginationSettings paginationSettings);
        Task<List<string>> GetPostcodesWithoutCityAsync();
        Task<List<string>> GetPostcodesWithoutDistrictAsync();
        Task<List<string>> GetPostcodesWithoutRegionAsync();
        Task ImportAupFromExcelAsync(Stream excelData);
        Task<byte[]> ExportAupToExcelAsync();
    }
}
