using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISPC.Models
{
    [Table("AssemblyTasks")]
    public class AssemblyTask
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public int AssemblerId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string AssemblerName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Ожидает";
        
        public DateTime StartDate { get; set; } = DateTime.Now;
        
        public DateTime? CompletionDate { get; set; }
        
        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Навигационные свойства
        public virtual Order Order { get; set; } = null!;
        public virtual Employee Assembler { get; set; } = null!;
    }
}