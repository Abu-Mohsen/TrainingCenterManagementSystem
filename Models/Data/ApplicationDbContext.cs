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

            // Constraint To Restrict The Same Student From Enrolling In The Same Course Multiple Times (pair Sid and Cid are Unique)
            modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

            // Configure the relationship between Instructor and Course
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull); // If an instructor is deleted, set the InstructorId in Course to null

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
