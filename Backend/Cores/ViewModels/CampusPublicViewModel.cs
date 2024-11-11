namespace Backend.Cores.ViewModels
{
    public class CampusPublicViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }   
}
