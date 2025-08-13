namespace Ankets.DTOs.Responses
{
    public class SurveyShareResponseDto
    {
        public int SurveyShareId { get; set; }
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public string ShareLink { get; set; }
    }
}