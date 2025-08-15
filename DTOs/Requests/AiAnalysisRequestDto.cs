namespace Ankets.DTOs.Requests
{
    public class AiAnalysisRequestDto
    {
        public int SurveyId { get; set; }
        public string? AnalysisResult { get; set; }
    }
}