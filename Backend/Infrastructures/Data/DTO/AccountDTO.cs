namespace Backend.Infrastructures.Data.DTO
{
    public class AccountDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Fullname {  get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone {  get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdated { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
