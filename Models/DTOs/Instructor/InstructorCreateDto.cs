using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Instructor
{
    public class InstructorCreateDto
    {
        [Required(ErrorMessage = "اسم المدرب مطلوب")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;
    }
}
