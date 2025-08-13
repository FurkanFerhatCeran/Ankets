namespace Ankets.DTOs.Requests
{
    public class QuestionAnswerRequestDto
    {
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? OptionId { get; set; }
    }
}