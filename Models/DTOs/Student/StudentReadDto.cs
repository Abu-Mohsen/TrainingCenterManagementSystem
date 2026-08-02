namespace TrainingCenterSystem.Models.DTOs.Student
{
    public class StudentReadDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int EnrolledCoursesCount { get; set; }

    }
}
