using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (category == null)
            {
                return NotFound();
            }
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
            if (!isUpdated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _surveyCategoryService.DeleteCategoryAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
