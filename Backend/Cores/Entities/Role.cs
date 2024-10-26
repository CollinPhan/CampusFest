using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    [Table("Role")]
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Name", TypeName="nvarchar(20)")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
