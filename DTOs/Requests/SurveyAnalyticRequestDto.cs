namespace Ankets.DTOs.Requests

{
    public class SurveyAnalyticRequestDto
    {
        public int SurveyId { get; set; }
        public string AnalyticType { get; set; }
        public string? Data { get; set; }
    }
}