﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Cores.Entities
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Username Is Required")]
        public string Username { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Password Is Required")]
        public string Password { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Email Is Required")]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Fullname { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public DateTime LastUpdatedTime { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
