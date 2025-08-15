namespace Ankets.DTOs.Requests
{
	public class ResetPasswordRequestDto
	{
		public required string Email { get; set; }
		public required string Token { get; set; } // Þifre sýfýrlama için kullanýlan özel token
		public required string NewPassword { get; set; }
	}
}