namespace Backend.Cores.ViewModels
{
    public class AccountPublicViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

    }
}
