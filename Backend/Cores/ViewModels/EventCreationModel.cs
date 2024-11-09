namespace Backend.Cores.ViewModels
{
    public class EventCreationModel
    {  
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Poster {  get; set; } = string.Empty;
        public int Capacity { get; set; }
        public long Price { get; set; }
        public DateOnly OnDate {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public Guid Club { set; get; }

    }
}
