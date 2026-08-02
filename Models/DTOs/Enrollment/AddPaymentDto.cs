using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Enrollment
{
    public class AddPaymentDto
    {
        [Required]
        public long EnrollmentId { get; set; } 

        [Required]
        [Range(0.01, 500, ErrorMessage = "يجب أدخال مبلغ دفعة صحيح ما بين 0.01 و 500 دينار")]
        public decimal Amount { get; set; }

        public string? Notes { get; set; }
    }
}
