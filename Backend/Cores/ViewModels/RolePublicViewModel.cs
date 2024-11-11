namespace Backend.Cores.ViewModels
{
    public class RoleViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public IEnumerable<string> Permission {get; set;} = new List<string>();
    }
}
