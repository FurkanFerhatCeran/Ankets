using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ankets.Services
{
    public interface ISurveyCategoryService
    {
        Task<IEnumerable<SurveyCategoryResponseDto>> GetAllCategoriesAsync();
        Task<SurveyCategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<SurveyCategoryResponseDto> CreateCategoryAsync(SurveyCategoryRequestDto requestDto);
        Task<bool> UpdateCategoryAsync(int id, SurveyCategoryRequestDto requestDto);
        Task<bool> DeleteCategoryAsync(int id);

        Task<IEnumerable<SurveyCategoryResponseDto>> ImportCategoriesAsync(List<SurveyCategoryRequestDto> categories);
    }
}
