using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;

namespace Ankets.Services
{
    // Kategori işlemleri için arayüz
    public interface ISurveyCategoryService
    {
        Task<IEnumerable<SurveyCategoryResponseDto>> GetAllCategoriesAsync();
        Task<SurveyCategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<SurveyCategoryResponseDto> CreateCategoryAsync(SurveyCategoryRequestDto requestDto);
        Task<bool> UpdateCategoryAsync(int id, SurveyCategoryRequestDto requestDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
