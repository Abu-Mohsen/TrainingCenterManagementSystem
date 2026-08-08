using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class AssignInstructorDto
    {
        [Required]
        public long InstructorId { get; set; }
    }
}
