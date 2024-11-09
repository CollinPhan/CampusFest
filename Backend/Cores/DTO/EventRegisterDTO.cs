namespace Backend.Cores.DTO
{
    public class EventRegisterDTO
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public Guid Account {  get; set; }

        public string Fullname { get; set; } = string.Empty;

        public DateTime RegisteredTime { get; set; }

        public DateTime? CheckinTime { get; set; }

        public string RegistrationStatus { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string OTP { get; set; } = string.Empty;
    }
}
