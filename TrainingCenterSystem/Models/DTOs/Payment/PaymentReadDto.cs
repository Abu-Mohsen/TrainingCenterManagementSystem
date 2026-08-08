namespace TrainingCenterSystem.Models.DTOs.Payment
{
    public class PaymentReadDto
    {
        public long Id { get; set; }
        public long EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }

    }
}
