using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterSystem.Models.Entities
{
    public class Course
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 500)]
        public int MaxCapacity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign key for the instructor teaching the course
        public long? InstructorId { get; set; }

        // Navigation property for the instructor teaching the course
        [ForeignKey(nameof(InstructorId))]
        public Instructor? Instructor { get; set; }

        // Navigation property for the enrollments in the course
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
