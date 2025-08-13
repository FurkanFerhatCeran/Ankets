namespace Ankets.DTOs.Responses
{
    public class SurveyResponseDto
    {
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}