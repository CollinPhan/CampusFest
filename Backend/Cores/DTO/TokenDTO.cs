namespace Backend.Cores.DTO
{
    public class TokenDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ForAccount { get; set; } = Guid.Empty;
        public string Reason { get; set; } = string.Empty;
        public string TokenValue { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime ExpirationTime { get; set; } = DateTime.UtcNow;
    }
}
