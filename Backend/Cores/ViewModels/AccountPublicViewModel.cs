namespace Backend.Cores.ViewModels
{
    public class AccountPublicViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;

        public int Campus {  get; set; } = 0;

        public Guid? Club {  get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
