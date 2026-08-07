namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class StudentEnrollmentDto
    {
        public long EnrollmentId { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }
}
