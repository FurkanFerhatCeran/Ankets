namespace Ankets.DTOs.Responses
{
    public class QuestionAnswerResponseDto
    {
        public int QuestionAnswerId { get; set; }
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? OptionId { get; set; }
    }
}