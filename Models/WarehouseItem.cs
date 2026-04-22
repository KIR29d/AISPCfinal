using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISPC.Models
{
    [Table("WarehouseItems")]
    public class WarehouseItem
    {
        [Key]
        public int Id { get; set; }
        
        public int ComponentId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ComponentName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        
        public int MinLevel { get; set; } = 5;
        
        [MaxLength(50)]
        public string Status { get; set; } = "В наличии";
        
        public DateTime LastMovement { get; set; } = DateTime.Now;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Навигационные свойства
        public virtual Component Component { get; set; } = null!;
    }
}