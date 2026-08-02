using System.ComponentModel.DataAnnotations;
namespace TrainingCenterSystem.Models.Entities
{
    public class Student
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;

        // Navigation Prop for student's Enrollments
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
