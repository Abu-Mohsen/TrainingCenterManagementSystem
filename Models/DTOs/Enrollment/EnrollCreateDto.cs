using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Enrollment
{
    public class EnrollCreateDto
    {
        [Required(ErrorMessage = "معرف الطالب مطلوب")]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "معرف الدورة مطلوب")]
        public long CourseId { get; set; }
    }
}
