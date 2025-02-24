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
    public class Patient
    {
        [Key]
        public int Id { get; set; } 

        public string ?Address { get; set; }
        public int? Age { get; set; }

        public string ?MedicalHistory { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }

}
