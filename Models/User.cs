using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISPC.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        public int RoleId { get; set; }
        
        public int? EmployeeId { get; set; }
        
        public int? ClientId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastLogin { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        public virtual Role Role { get; set; } = null!;
    }
}