namespace TrainingCenterSystem.Models.DTOs.Course
{
    public class CourseReadDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public decimal Fee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public long? InstructorId { get; set; }

        public string? InstructorName { get; set; }

        public int EnrolledStudentsCount { get; set; } // Number of students enrolled in the course

    }
}
