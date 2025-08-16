using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ankets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SurveysController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Kullanýcý ID'si bulunamadý.");
            }
            return int.Parse(userId);
        }

        // GET: api/Surveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetSurveys()
        {
            var creatorId = GetUserId();
            var surveys = await _surveyService.GetSurveysByCreatorIdAsync(creatorId);
            return Ok(surveys);
        }

        // GET: api/Surveys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> GetSurvey(int id)
        {
            var creatorId = GetUserId();
            var survey = await _surveyService.GetSurveyByIdAsync(id, creatorId);

            if (survey == null)
            {
                return NotFound("Anket bulunamadý.");
            }

            return Ok(survey);
        }

        // POST: api/Surveys
        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> CreateSurvey([FromBody] SurveyRequestDto surveyRequestDto)
        {
            var creatorId = GetUserId();
            var newSurvey = await _surveyService.CreateSurveyAsync(surveyRequestDto, creatorId);

            return CreatedAtAction(nameof(GetSurvey), new { id = newSurvey.SurveyId }, newSurvey);
        }

        // PUT: api/Surveys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(int id, [FromBody] SurveyRequestDto surveyRequestDto)
        {
            var creatorId = GetUserId();
            var isUpdated = await _surveyService.UpdateSurveyAsync(id, surveyRequestDto, creatorId);

            if (!isUpdated)
            {
                return NotFound("Güncellenecek anket bulunamadý.");
            }

            return NoContent();
        }

        // DELETE: api/Surveys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var creatorId = GetUserId();
            var isDeleted = await _surveyService.DeleteSurveyAsync(id, creatorId);

            if (!isDeleted)
            {
                return NotFound("Silinecek anket bulunamadý.");
            }

            return NoContent();
        }
    }
}
