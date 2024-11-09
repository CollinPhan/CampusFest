using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    public class Campus
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campus name is required")]
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        public virtual IEnumerable<Club> Clubs { get; set; } = new List<Club>();

        public virtual IEnumerable<Account> Accounts { get; set; } =new List<Account>();

    }
}
