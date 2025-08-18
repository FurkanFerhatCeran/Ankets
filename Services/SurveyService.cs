using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ankets.Data;
using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ankets.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyResponseDto>> GetSurveysByCreatorIdAsync(int creatorId);
        Task<SurveyResponseDto?> GetSurveyByIdAsync(int surveyId, int creatorId);
        Task<SurveyResponseDto> CreateSurveyAsync(SurveyRequestDto surveyRequestDto, int creatorId);
        Task<bool> UpdateSurveyAsync(int surveyId, SurveyRequestDto surveyRequestDto, int creatorId);
        Task<bool> DeleteSurveyAsync(int surveyId, int creatorId);
    }

    public class SurveyService : ISurveyService
    {
        private readonly AppDbContext _context;
        private readonly IPermissionService _permissionService;

        public SurveyService(AppDbContext context, IPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<SurveyResponseDto>> GetSurveysByCreatorIdAsync(int creatorId)
        {
            var isAdmin = await _permissionService.IsAdminAsync(creatorId);
            var query = _context.Surveys.AsNoTracking();
            if (!isAdmin)
            {
                query = query.Where(s => s.CreatorId == creatorId);
            }

            var surveys = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
            var result = new List<SurveyResponseDto>();
            foreach (var s in surveys)
            {
                var myRole = await _permissionService.GetMyRoleForSurveyAsync(s.SurveyId, creatorId);
                result.Add(ToDto(s, myRole, creatorId));
            }
            return result;
        }

        public async Task<SurveyResponseDto?> GetSurveyByIdAsync(int surveyId, int creatorId)
        {
            var s = await _context.Surveys.AsNoTracking().FirstOrDefaultAsync(x => x.SurveyId == surveyId);
            if (s == null) return null;
            var myRole = await _permissionService.GetMyRoleForSurveyAsync(surveyId, creatorId);
            return ToDto(s, myRole, creatorId);
        }

        public async Task<SurveyResponseDto> CreateSurveyAsync(SurveyRequestDto surveyRequestDto, int creatorId)
        {
            var survey = new Survey
            {
                SurveyTitle = surveyRequestDto.Title,
                SurveyDescription = surveyRequestDto.Description,
                CategoryId = surveyRequestDto.CategoryId == 0 ? null : surveyRequestDto.CategoryId,
                CreatorId = creatorId
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return ToDto(survey, "owner", creatorId);
        }

        public async Task<bool> UpdateSurveyAsync(int surveyId, SurveyRequestDto surveyRequestDto, int creatorId)
        {
            if (!await _permissionService.CanManageSurveyAsync(surveyId, creatorId)) return false;

            var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.SurveyId == surveyId);
            if (survey == null) return false;

            survey.SurveyTitle = surveyRequestDto.Title;
            survey.SurveyDescription = surveyRequestDto.Description;
            survey.CategoryId = surveyRequestDto.CategoryId == 0 ? null : surveyRequestDto.CategoryId;
            survey.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSurveyAsync(int surveyId, int creatorId)
        {
            if (!await _permissionService.CanManageSurveyAsync(surveyId, creatorId)) return false;

            var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.SurveyId == surveyId);
            if (survey == null) return false;

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return true;
        }

        private static SurveyResponseDto ToDto(Survey s, string myRole, int userId)
        {
            return new SurveyResponseDto
            {
                SurveyId = s.SurveyId,
                Title = s.SurveyTitle,
                Description = s.SurveyDescription ?? string.Empty,
                CategoryId = s.CategoryId ?? 0,
                CreatorId = s.CreatorId,
                IsOwner = s.CreatorId == userId,
                MyRole = myRole
            };
        }
    }
}
