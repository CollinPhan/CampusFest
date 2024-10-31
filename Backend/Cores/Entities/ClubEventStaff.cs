using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    public class ClubEventStaff
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Club))]
        [Required]
        public Guid ClubId { get; set; }

        [ForeignKey(nameof(Account))]
        [Required]
        public Guid AccountId { get; set; }

        public virtual Club Club { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
    }
}
