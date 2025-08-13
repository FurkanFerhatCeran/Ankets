namespace Ankets.DTOs.Requests
{
    public class SurveyShareRequestDto
    {
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public string ShareLink { get; set; }
    }
}