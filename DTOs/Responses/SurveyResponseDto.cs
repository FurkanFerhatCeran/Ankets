namespace Ankets.DTOs.Responses
{
	public class SurveyResponseDto
	{
		public int SurveyId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int CategoryId { get; set; }

		public int CreatorId { get; set; }
		public bool IsOwner { get; set; }
		public string MyRole { get; set; }
	}
}