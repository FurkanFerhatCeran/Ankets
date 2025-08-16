using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;

namespace Ankets.Services
{
    // �� mant��� i�in servis aray�z�
    public interface ISurveyService
    {
        // Yetkili kullan�c�ya ait t�m anketleri d�nd�r�r
        Task<IEnumerable<SurveyResponseDto>> GetSurveysByCreatorIdAsync(int creatorId);

        // Belirli bir anketi d�nd�r�r
        Task<SurveyResponseDto> GetSurveyByIdAsync(int surveyId, int creatorId);

        // Yeni bir anket olu�turur
        Task<SurveyResponseDto> CreateSurveyAsync(SurveyRequestDto surveyRequestDto, int creatorId);

        // Var olan bir anketi g�nceller
        Task<bool> UpdateSurveyAsync(int surveyId, SurveyRequestDto surveyRequestDto, int creatorId);

        // Var olan bir anketi siler
        Task<bool> DeleteSurveyAsync(int surveyId, int creatorId);
    }
}
