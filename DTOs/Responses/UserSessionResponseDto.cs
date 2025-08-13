namespace Ankets.DTOs.Responses
{
    public class UserSessionResponseDto
    {
        public int UserSessionId { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; }
        public string? IpAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}   