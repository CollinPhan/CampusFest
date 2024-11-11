namespace Backend.Cores.ViewModels
{
    public class EventUpdateModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string PosterUrl {  get; set; } = string.Empty;

        public DateOnly OnDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
