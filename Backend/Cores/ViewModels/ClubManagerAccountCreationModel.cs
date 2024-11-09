namespace Backend.Cores.ViewModels
{
    public class ClubManagerAccountCreationModel
    {
        // Club data
        public string ClubName { get; set; } = null!;

        // Account data
        public string Fullname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        // Data that will be used by both
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Campus {  get; set; } = 0;
    }
}
