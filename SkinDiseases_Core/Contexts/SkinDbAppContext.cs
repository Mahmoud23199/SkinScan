using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkinScan_Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_Core.Contexts
{
    public class SkinDbAppContext: IdentityDbContext
    {
        public SkinDbAppContext(DbContextOptions<SkinDbAppContext> options) : base(options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PationDiagnosis> PationDiagnosis { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<ChatbotQuestion> ChatbotQuestion { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<ChatbotQuestion>()
                .HasOne(q => q.NextQuestion)
                .WithMany(q => q.InverseNextQuestion)
                .HasForeignKey(q => q.NextQuestionId)
                .OnDelete(DeleteBehavior.Restrict);  

             
            modelBuilder.Entity<ChatbotQuestion>()
                .HasOne(q => q.Parent)
                .WithMany(q => q.InverseParent)
                .HasForeignKey(q => q.ParentId)
                .OnDelete(DeleteBehavior.Restrict);  
        }
    }
}
