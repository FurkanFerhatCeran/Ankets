namespace Ankets.DTOs.Requests
{
    public class SurveyResponseRequestDto
    {
        public int SurveyId { get; set; }
        public int? RespondentId { get; set; }
        public string SessionId { get; set; }
        public string? IpAddress { get; set; }
    }
}