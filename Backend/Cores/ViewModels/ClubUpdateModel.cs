namespace Backend.Cores.ViewModels
{
    public class ClubUpdateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Campus { get; set; } 
    }
}
