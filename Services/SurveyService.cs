using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;

namespace Ankets.Services
{
    // Ýþ mantýðý için servis arayüzü
    public interface ISurveyService
    {
        // Yetkili kullanýcýya ait tüm anketleri döndürür
        Task<IEnumerable<SurveyResponseDto>> GetSurveysByCreatorIdAsync(int creatorId);

        // Belirli bir anketi döndürür
        Task<SurveyResponseDto> GetSurveyByIdAsync(int surveyId, int creatorId);

        // Yeni bir anket oluþturur
        Task<SurveyResponseDto> CreateSurveyAsync(SurveyRequestDto surveyRequestDto, int creatorId);

        // Var olan bir anketi günceller
        Task<bool> UpdateSurveyAsync(int surveyId, SurveyRequestDto surveyRequestDto, int creatorId);

        // Var olan bir anketi siler
        Task<bool> DeleteSurveyAsync(int surveyId, int creatorId);
    }
}
