using Common.Dtos;
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
        public async Task<ActionResult<List<AupDto>>> GetAups([FromQuery]PaginationSettings paginationSettings)
        {
            return Ok(await _postcodeService.GetAupsAsync(paginationSettings));
        }

        [HttpGet]
        public async Task<ActionResult<List<AupDto>>> GetPostcodesWithoutCity()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutCityAsync());
        }

        [HttpGet]
        public async Task<ActionResult<List<AupDto>>> GetPostcodesWithoutDistrict()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutDistrictAsync());
        }

        [HttpGet]
        public async Task<ActionResult<List<AupDto>>> GetPostcodesWithoutRegion()
        {
            return Ok(await _postcodeService.GetPostcodesWithoutRegionAsync());
        }

        [HttpPost]
        public async Task<ActionResult> ImportAupFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            await _postcodeService.ImportAupFromExcelAsync(file);
            return Ok();
        }

        [HttpGet]
        public async Task<FileContentResult> ExportAupToExcel()
        {
            var fileContent = await _postcodeService.ExportAupToExcelAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AUP.xlsx");
        }
    }
}
