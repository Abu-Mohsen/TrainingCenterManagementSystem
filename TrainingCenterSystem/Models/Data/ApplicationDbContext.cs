using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Entities;

namespace TrainingCenterSystem.Models.Data
{
    public class ApplicationDbContext : DbContext // Inherit from [DbContext] to create a database context for Entity Framework Core
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Our DbSets represent the [tables in the database]. Each DbSet corresponds to an entity class.
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and constraints using Fluent API if needed

            // Prevent the same student from enrolling in the same course twice (DB-level guarantee)
            modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            // Unique Indexes for Email - Prevent duplicate emails at the database level, not just in application code

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Instructor>()
                .HasIndex(i => i.Email)
                .IsUnique();

            // Instructor <-> Course: deleting an instructor un-assigns their courses (does not delete them)

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            /*
             * Student <-> Enrollment: made EXPLICIT 
             * The controller only ever allows deleting a student when no ACTIVE enrollment remains
             * this Cascade only ever removes already-Canceled enrollment rows bla bla bla
             * */
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            //Enrollment <->Payment: if an enrollment row is ever removed, its payments go with it
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Enrollment)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // Precision For Decimal Properties (Course | Payment | Enrollment)
            modelBuilder.Entity<Course>()
                .Property(c => c.Fee)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.TotalAmount)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.PaidAmount)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(8, 2);

        }
    }
}
