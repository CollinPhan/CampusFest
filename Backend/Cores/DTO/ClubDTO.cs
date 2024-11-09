namespace Backend.Cores.DTO
{
    public class ClubDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Avatar {  get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int CampusId { get; set; }

        public string CampusName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }
}
