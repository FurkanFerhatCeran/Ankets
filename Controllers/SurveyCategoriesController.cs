using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml; // EPPlus
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ankets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyCategoriesController : ControllerBase
    {
        private readonly ISurveyCategoryService _surveyCategoryService;

        public SurveyCategoriesController(ISurveyCategoryService surveyCategoryService)
        {
            _surveyCategoryService = surveyCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyCategoryResponseDto>>> GetCategories()
        {
            var categories = await _surveyCategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyCategoryResponseDto>> GetCategory(int id)
        {
            var category = await _surveyCategoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<SurveyCategoryResponseDto>> CreateCategory(SurveyCategoryRequestDto requestDto)
        {
            var newCategory = await _surveyCategoryService.CreateCategoryAsync(requestDto);
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.CategoriesId }, newCategory);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(int id, SurveyCategoryRequestDto requestDto)
        {
            var isUpdated = await _surveyCategoryService.UpdateCategoryAsync(id, requestDto);
            if (!isUpdated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _surveyCategoryService.DeleteCategoryAsync(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }

        // JSON ile import
        [HttpPost("import")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ImportCategories([FromBody] SurveyCategoryImportDto importDto)
        {
            if (importDto?.Categories == null || !importDto.Categories.Any())
                return BadRequest("Kategori listesi boş.");

            var createdCategories = await _surveyCategoryService.ImportCategoriesAsync(importDto.Categories);
            return Ok(createdCategories);
        }

        // Excel ile import
        [HttpPost("import-excel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ImportCategoriesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var categories = new List<SurveyCategoryRequestDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // 1.satır başlık
                    {
                        var name = worksheet.Cells[row, 1].Text;
                        var description = worksheet.Cells[row, 2].Text;
                        var color = worksheet.Cells[row, 3].Text;

                        if (string.IsNullOrEmpty(name)) continue;

                        categories.Add(new SurveyCategoryRequestDto
                        {
                            CategoriesName = name,
                            CategoriesDescription = string.IsNullOrEmpty(description) ? null : description,
                            Color = string.IsNullOrEmpty(color) ? null : color
                        });
                    }
                }
            }

            var createdCategories = await _surveyCategoryService.ImportCategoriesAsync(categories);
            return Ok(createdCategories);
        }
    }
}
