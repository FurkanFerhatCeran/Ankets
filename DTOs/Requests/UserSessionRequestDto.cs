namespace Ankets.DTOs.Requests
{
    public class UserSessionRequestDto
    {
        public int UserId { get; set; }
        public string SessionToken { get; set; }
        public string? IpAddress { get; set; }
    }
}