﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Cores.Entities
{
    public class Club
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(AllowEmptyStrings = false, ErrorMessage = "Club name is required")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage ="Club contact email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Club campus is required")]
        [ForeignKey(nameof(Campus))]
        public int CampusId { get; set; }

        [Required(ErrorMessage = "Club must have a manager")]
        [ForeignKey(nameof(Account))]
        public Guid ManagerId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public virtual Account Manager { get; set; } = null!;

        public virtual Campus Campus { get; set; } = null!;

        public virtual IEnumerable<ClubEventStaff> Staffs { get; set; } = Enumerable.Empty<ClubEventStaff>();

        public virtual IEnumerable<Event> Events { get; set; } = Enumerable.Empty<Event>();
    }
}