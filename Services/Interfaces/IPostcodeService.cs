using Common.Dtos;
using Common.Models;
using Microsoft.AspNetCore.Http;

namespace Services.Interfaces
{
    public interface IPostcodeService
    {
        Task<List<AupDto>> GetAupsAsync(PaginationSettings paginationSettings);
        Task<List<string>> GetPostcodesWithoutCityAsync();
        Task<List<string>> GetPostcodesWithoutDistrictAsync();
        Task<List<string>> GetPostcodesWithoutRegionAsync();
        Task ImportAupFromExcelAsync(IFormFile file);
        Task<byte[]> ExportAupToExcelAsync();
    }
}
