namespace Backend.Cores.ViewModels
{
    public class AccountCreationModel
    {
        public string Username { get; set; } = null!;
        
        public string Fullname { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
