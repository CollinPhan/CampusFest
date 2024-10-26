namespace Backend.Cores.ViewModels
{
    public class AccountUpdateModel
    {
        public Guid AccountId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Fullname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
