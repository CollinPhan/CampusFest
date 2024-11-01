namespace Backend.Cores.DTO
{
    public class EventDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public long Price { get; set; } = 0L;

        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);


        public TimeOnly StartTime = TimeOnly.FromDateTime(DateTime.UtcNow);


        public TimeOnly EndTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1));

        public int Capacity { get; set; } = 0;

        public Guid Club { get; set; }
    }
}
