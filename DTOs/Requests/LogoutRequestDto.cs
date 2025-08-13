namespace Ankets.DTOs.Requests
{
    public class LogoutRequestDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}