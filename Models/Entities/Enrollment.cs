using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterSystem.Models.Entities
{
    public class Enrollment
    {
        [Key]
        public long Id { get; set; }

        // Foreign key for the student enrolled in the course
        [Required]
        public long StudentId { get; set; }

        // Navigation property for the student enrolled in the course
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        // Foreign key for the course the student is enrolled in
        [Required]
        public long CourseId { get; set; }

        // Navigation property for the course the student is enrolled in
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        public bool IsFullyPaid => PaidAmount >= TotalAmount;

        // Navigation property for the payments related to this enrollment
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
