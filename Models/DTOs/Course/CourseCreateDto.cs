using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "عنوان الدورة مطلوب")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "السعة يجب أن تكون بين 1 و 500")]
        public int MaxCapacity { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "الرسوم يجب أن تكون قيمة موجبة بين 0 و 500 دينار")]
        public decimal Fee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public long? InstructorId { get; set; }
    }
}
