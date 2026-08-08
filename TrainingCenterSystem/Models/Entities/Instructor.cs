using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.Entities
{
    public class Instructor
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)] // Note For Me : In Entity Use MaxLength and in DTO Use StringLength
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string Email { get; set; } = string.Empty;

        
        [StringLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        // Navigation property for the courses taught by the instructor
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
