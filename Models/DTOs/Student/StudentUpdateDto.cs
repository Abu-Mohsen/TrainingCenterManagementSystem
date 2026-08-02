using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Student
{
    public class StudentUpdateDto : StudentCreateDto
    {
        [Required(ErrorMessage = "معرف الطالب مطلوب للتحديث")]
        public long Id { get; set; } // Include the Id for update operations
    }
}
