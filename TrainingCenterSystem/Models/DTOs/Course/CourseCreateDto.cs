using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Course title is required.")]
        [StringLength(100 , ErrorMessage = "Course title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Maximum capacity is required.")]
        [Range(20, 300, ErrorMessage = "Capacity must be between 20 and 300 students.")]
        public int MaxCapacity { get; set; }

        [Required(ErrorMessage = "Course fee is required.")]
        [Range(0, 500, ErrorMessage = "Fee must be a positive value between 0 and 500.")]
        public decimal Fee { get; set; } 

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public long? InstructorId { get; set; }
    }
}
