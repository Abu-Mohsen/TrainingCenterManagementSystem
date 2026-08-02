namespace TrainingCenterSystem.Models.DTOs.Enrollment
{
    public class EnrollmentReadDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;

         
        // 
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public bool IsFullyPaid { get; set; }

    }
}
