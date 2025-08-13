namespace Ankets.DTOs.Responses
{
    public class SurveyAnalyticResponseDto
    {
        public int SurveyAnalyticId { get; set; }
        public int SurveyId { get; set; }
        public string AnalyticType { get; set; }
        public string? Data { get; set; }
    }
}