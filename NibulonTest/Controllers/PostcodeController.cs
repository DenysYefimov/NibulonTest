using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace NibulonTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostcodeController : ControllerBase
    {
        private readonly IPostcodeService _postcodeService;

        public PostcodeController(IPostcodeService postcodeService)
        {
            _postcodeService = postcodeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAups([FromQuery]PaginationSettings paginationSettings)
        {
            return Ok(await _postcodeService.GetAupsAsync(paginationSettings));
        }

        [HttpGet]
        public async Task<IActionResult> GetPostcodesWithoutCity()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutCityAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPostcodesWithoutDistrict()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutDistrictAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPostcodesWithoutRegion()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutRegionAsync());
        }

        [HttpPost]
        public async Task<IActionResult> ImportAupFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            await _postcodeService.ImportAupFromExcelAsync(file);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ExportAupToExcel()
        {
            var fileContent = await _postcodeService.ExportAupToExcelAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AUP.xlsx");
        }
    }
}
