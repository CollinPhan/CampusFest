namespace Backend.Cores.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IEnumerable<string> Permission { get; set; } = new List<string>();
    }
}
