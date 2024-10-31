using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    public class EventRegistration
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Event))]
        public Guid EventId { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string RegistrationStatus { get; set; } = string.Empty;

        public string QRCode { get; set; } = string.Empty;

        public string OTP {  get; set; } = string.Empty;


        public DateTime RegisteredTime { get; set; }

        public DateTime CheckinTime { get; set; }

        public virtual Account Account { get; set; } = null!;

        public virtual Event Event { get; set; } = null!;
    }
}
