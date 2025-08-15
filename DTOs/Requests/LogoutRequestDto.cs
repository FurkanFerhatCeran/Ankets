namespace Ankets.DTOs.Requests
{
    public class LogoutRequestDto
    {
        public string? RefreshToken { get; set; }
        public bool LogoutFromAllDevices { get; set; } = false;
    }
}