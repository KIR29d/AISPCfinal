using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AISPC.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ClientType { get; set; } // Физическое лицо, ИП, ООО

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(20)]
        public string INN { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}