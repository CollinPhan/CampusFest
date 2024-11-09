namespace Backend.Cores.ViewModels
{
    public class EventViewModel
    {
        public Guid Id { get; set; }

        public string Image {  get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        public string Location { get; set; } = string.Empty;

        public long Price { get; set; } 

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public int Capacity { get; set; }

        public Guid OperatorId { get; set; }

        public Guid Club {  get; set; }

        public int Campus { get; set; }
    }
}
