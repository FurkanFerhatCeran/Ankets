namespace Ankets.DTOs.Responses
{
    public class AuditLogResponseDto
    {
        public int AuditLogId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}