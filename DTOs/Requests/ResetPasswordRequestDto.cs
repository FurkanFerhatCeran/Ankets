namespace Ankets.DTOs.Requests
{
	public class ResetPasswordRequestDto
	{
		public required string Email { get; set; }
		public required string Token { get; set; } // �ifre s�f�rlama i�in kullan�lan �zel token
		public required string NewPassword { get; set; }
	}
}