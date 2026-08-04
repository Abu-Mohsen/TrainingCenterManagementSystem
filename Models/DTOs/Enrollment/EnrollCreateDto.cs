using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Enrollment
{
    public class EnrollCreateDto
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public long CourseId { get; set; }
    }
}
