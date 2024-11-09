namespace Backend.Cores.DTO
{
    public class AccountDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Fullname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int? Campus { get; set; } = null;

        public Guid? Club { get; set; } = null;

        public bool IsVerified { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdated { get; set; }

        public int RoleId { get; set; } = 0;

        public string Role { get; set; } = string.Empty;
    }
}
