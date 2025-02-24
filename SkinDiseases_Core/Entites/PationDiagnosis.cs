using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_Core.Entites
{
    public class PationDiagnosis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } 

        public string? ImagePath { get; set; } 

        public string? Diagnosis { get; set; } 

        public string? Details { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
