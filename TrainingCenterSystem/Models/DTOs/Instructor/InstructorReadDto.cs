namespace TrainingCenterSystem.Models.DTOs.Instructor
{
    public class InstructorReadDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int AssignedCoursesCount { get; set; } // Number of courses the instructor is teaching
    }
}
