using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    public class Token
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;

        [Required]
        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Required]
        [ForeignKey(nameof(Account))]
        public Guid ValidAccount {  get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
