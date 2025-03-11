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
    
    public class Disease
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Symptoms { get; set; }

        public string? Causes { get; set; }

        public string? Treatments { get; set; }

        public string? References1 { get; set; }
    }

}
