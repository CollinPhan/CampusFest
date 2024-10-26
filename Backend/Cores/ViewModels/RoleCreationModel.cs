namespace Backend.Cores.ViewModels
{
    public class RoleCreationModel
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<int> Permission { get; set; } = new List<int>();
    }
}
