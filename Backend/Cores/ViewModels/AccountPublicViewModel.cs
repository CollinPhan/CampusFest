namespace Backend.Cores.ViewModels
{
    public class AccountPublicViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }

    }
}
