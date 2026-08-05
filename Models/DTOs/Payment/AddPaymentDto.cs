using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Payment
{
    public class AddPaymentDto
    {
        [Required(ErrorMessage = "Enrollment ID is required.")]
        public long EnrollmentId { get; set; }

        [Required(ErrorMessage = "Payment amount is required.")]
        [Range(30, 1000, ErrorMessage = "Payment amount must start from 30.")]
        public decimal Amount { get; set; } = 0;

        public string? Notes { get; set; }
    }
}
