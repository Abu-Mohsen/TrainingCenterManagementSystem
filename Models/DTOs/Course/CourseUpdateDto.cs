using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class CourseUpdateDto : CourseCreateDto
    {
        [Required(ErrorMessage = "معرف الدورة مطلوب للتحديث")]
        public long Id { get; set; } // Include the Id for update operations
    
    }
}
