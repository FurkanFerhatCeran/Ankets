namespace Ankets.DTOs.Requests
{
    public class QuestionRequestDto
    {
        public int SurveyId { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
    }
}