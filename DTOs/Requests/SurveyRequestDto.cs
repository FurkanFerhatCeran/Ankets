namespace Ankets.DTOs.Requests
{
    public class SurveyRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}