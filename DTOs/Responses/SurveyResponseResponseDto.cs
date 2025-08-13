namespace Ankets.DTOs.Responses
{
    public class SurveyResponseResponseDto
    {
        public int ResponseId { get; set; }
        public int SurveyId { get; set; }
        public int? RespondentId { get; set; }
        public string SessionId { get; set; }
        public string? IpAddress { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPartial { get; set; }
        public decimal CompletionPercentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}