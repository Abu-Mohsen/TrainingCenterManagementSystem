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

        // "Active" = has at least one course currently in progress (StartDate <= today <= EndDate).
        // "Inactive" = no course, or none currently in progress. Computed on read - never stored.
        public string Status { get; set; } = string.Empty;
    }
}
