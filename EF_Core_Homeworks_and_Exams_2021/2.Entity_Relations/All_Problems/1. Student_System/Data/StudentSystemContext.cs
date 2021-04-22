using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        // needed by Judge
        public StudentSystemContext(DbContextOptions options) 
            : base(options)
        {

        }

        // DbSets
        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }


        // OnConfiguring and OnModelCreating
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Settings.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(course =>
            {
                //CourseId - PK
                course.HasKey(c => c.CourseId);

                //Name(up to 80 characters, unicode)
                course
                    .Property(c => c.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);

                //Description(unicode, not required)
                course
                    .Property(c => c.Description)
                    .IsRequired(false)
                    .IsUnicode(true);

                //StartDate
                course
                    .Property(c => c.StartDate)
                    .HasColumnType("DATETIME2")
                    .IsRequired(true);

                //EndDate
                course
                   .Property(c => c.EndDate)
                   .HasColumnType("DATETIME2")
                   .IsRequired(true);

                //Price
                course
                    .Property(c => c.Price)
                    .HasColumnType("Decimal(18,2)")
                    .IsRequired(true);
            });

            modelBuilder.Entity<Homework>(homeworks =>
            {
                // HomeworkId - PK
                homeworks.HasKey(h => h.HomeworkId);

                // Content(string, linking to a file, not unicode)
                homeworks
                    .Property(h => h.Content)
                    .IsRequired(true)
                    .IsUnicode(false);

                // ContentType(enum – can be Application, Pdf or Zip) -> to move into Enumerations folder if necessary
                //homeworks
                //    .Property(h => h.ContentType)
                //    .IsRequired(true);

                // SubmissionTime
                homeworks
                    .Property(h => h.SubmissionTime)
                    .HasColumnType("DATETIME2")
                    .IsRequired(true);

                // StudentId
                homeworks
                    .HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(h => h.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CourseId
                homeworks
                    .HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(h => h.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Resource>(resources =>
            {
                //ResourceId
                resources.HasKey(r => r.ResourceId);

                //Name(up to 50 characters, unicode)
                resources
                    .Property(r => r.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);

                //Url(not unicode)
                resources
                    .Property(r => r.Url)
                    .HasMaxLength(250) // not obligatory
                    .IsRequired(true)
                    .IsUnicode(true);

                //ResourceType(enum – can be Video, Presentation, Document or Other)
                //resources
                //    .Property()

                //CourseId
                resources
                    .HasOne(r => r.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(r => r.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(students =>
            {
                //StudentId
                students.HasKey(s => s.StudentId);

                //Name(up to 100 characters, unicode)
                students
                    .Property(s => s.Name)
                    .HasMaxLength(100)
                    .IsRequired(true)
                    .IsUnicode(true);

                //PhoneNumber(exactly 10 characters, not unicode, not required)
                students
                    .Property(s => s.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnType("CHAR")
                    .IsUnicode(false)
                    .IsRequired(false);

                //RegisteredOn
                students
                    .Property(s => s.RegisteredOn)
                    .HasColumnType("DATETIME2")
                    .IsRequired(true);

                //Birthday(not required)
                students
                    .Property(s => s.Birthday)
                    .HasColumnType("DATE")
                    .IsRequired(false);
            });

            modelBuilder.Entity<StudentCourse>(studentCourses =>
            {
                //mapping class between Students and Courses
                studentCourses.HasKey(sc => new { sc.StudentId, sc.CourseId });

                //Students
                studentCourses
                    .HasOne(sc => sc.Student)
                    .WithMany(s => s.CourseEnrollments)
                    .HasForeignKey(sc => sc.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                //Courses
                studentCourses
                    .HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentsEnrolled)
                    .HasForeignKey(sc => sc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
