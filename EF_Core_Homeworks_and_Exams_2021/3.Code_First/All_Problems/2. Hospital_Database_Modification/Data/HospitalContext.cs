using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
            : base(options)
        {

        }

        // DbSets - here
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Doctor> Doctor { get; set; }


        // OnConfigurin and OnModelCreating
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataSettings.ConnectionString);
            }
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Patient>(patient =>
                {
                    //Primary Key
                    patient.HasKey(p => p.PatientId);

                    //	FirstName (up to 50 characters, unicode)
                    patient
                        .Property(p => p.FirstName)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    //	LastName (up to 50 characters, unicode)
                    patient
                        .Property(p => p.LastName)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    //	Address (up to 250 characters, unicode)
                    patient
                        .Property(p => p.Address)
                        .HasMaxLength(250)
                        .IsRequired(true)
                        .IsUnicode(true);

                    //	Email (up to 80 characters, not unicode)
                    patient
                        .Property(p => p.Email)
                        .HasMaxLength(80)
                        .IsRequired(true)
                        .IsUnicode(false);

                    //	HasInsurance
                    patient
                        .Property(p => p.HasInsurance)
                        .IsRequired(true);
                });

            modelBuilder
                .Entity<Diagnose>(diagnose =>
                {
                    // DiagnoseId - PK
                    diagnose.HasKey(d => d.DiagnoseId);

                    // Name(up to 50 characters, unicode)
                    diagnose
                        .Property(d => d.Name)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);

                    // Comments(up to 250 characters, unicode)
                    diagnose
                        .Property(d => d.Comments)
                        .HasMaxLength(250)
                        .IsRequired(true)
                        .IsUnicode(true);

                    // Patient - One to Many
                    diagnose
                        .HasOne(d => d.Patient)
                        .WithMany(p => p.Diagnoses)
                        .HasForeignKey(d => d.PatientId)
                        .OnDelete(DeleteBehavior.Restrict);

                });

            modelBuilder
                .Entity<Visitation>(visitation =>
                {
                    // PKey
                    visitation.HasKey(v => v.VisitationId);

                    // Date
                    visitation
                        .Property(v => v.Date)
                        .HasColumnType("DATETIME2")
                        .IsRequired(true);

                    //	Comments(up to 250 characters, unicode)
                    visitation
                        .Property(v => v.Comments)
                        .HasMaxLength(250)
                        .IsRequired(true)
                        .IsUnicode(true);

                    //	Patient
                    visitation
                        .HasOne(v => v.Patient)
                        .WithMany(p => p.Visitations)
                        .HasForeignKey(v => v.ParientId);

                    //Doctor
                    visitation
                        .HasOne(v => v.Doctor)
                        .WithMany(d => d.Visitations)
                        .HasForeignKey(v => v.DoctorId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder
                .Entity<Medicament>(medicaments =>
                {
                    //	MedicamentId
                    medicaments.HasKey(m => m.MedicamentId);

                    //	Name(up to 50 characters, unicode)
                    medicaments
                        .Property(m => m.Name)
                        .HasMaxLength(50)
                        .IsRequired(true)
                        .IsUnicode(true);
                });

            modelBuilder.Entity<PatientMedicament>(patientMedicament =>
            {
                patientMedicament
                    .HasKey(pm => new { pm.PatientId, pm.MedicamentId });

                patientMedicament
                    .HasOne(pm => pm.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(pm => pm.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                patientMedicament
                    .HasOne(pm => pm.Medicament)
                    .WithMany(m => m.Prescriptions)
                    .HasForeignKey(pm => pm.MedicamentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Doctor>(doctor =>
            {
                doctor.HasKey(d => d.DoctorId);

                doctor
                    .Property(d => d.Name)
                    .HasMaxLength(100)
                    .IsRequired(true)
                    .IsUnicode(true);

                doctor
                    .Property(d => d.Specialty)
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsUnicode(true);

            });
        }
    }
}
