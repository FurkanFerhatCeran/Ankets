namespace Ankets.DTOs.Requests
{
    public class AuditLogRequestDto
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}