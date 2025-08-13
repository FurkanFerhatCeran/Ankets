namespace Ankets.DTOs.Responses
{
    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
    }
}