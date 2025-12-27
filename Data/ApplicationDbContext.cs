using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tadawi.Models;

namespace Tadawi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<DiseaseType> DiseaseTypes { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientFile> PatientFiles { get; set; }
        public DbSet<PeriodicDispense> PeriodicDispenses { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Visit> Visits { get; set; }

    }
}
