using Microsoft.AspNetCore.Identity;
using SkinScan_Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_Core.Contexts
{
    public class ApplicationUser : IdentityUser
    {
        public UserType UserType { get; set; }
        public ICollection<PationDiagnosis> Diagnoses { get; set; } = new List<PationDiagnosis>();
    }
    public enum UserType
    {
        Patient, Doctor
    }
}
