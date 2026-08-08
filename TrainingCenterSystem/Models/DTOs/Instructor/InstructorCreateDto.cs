using System.ComponentModel.DataAnnotations;

namespace TrainingCenterSystem.Models.DTOs.Instructor
{
    public class InstructorCreateDto
    {
        [Required(ErrorMessage = "Instructor name is required.")]
        [StringLength(100 , ErrorMessage = "Instructor name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(40 , ErrorMessage = " Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string Specialization { get; set; } = string.Empty;
    }
}
