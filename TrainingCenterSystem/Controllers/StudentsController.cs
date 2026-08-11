using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.DTOs.Student;
using TrainingCenterSystem.Models.Entities;


namespace TrainingCenterSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // Depen
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ByCriteria")]
        public async Task<IActionResult> GetStudents([FromQuery] StudentSearchCriteriaDto dto)
        {
            dto.Name = dto.Name?.Trim();
            dto.Specialty = dto.Specialty?.Trim();

            var query = _context.Students.AsQueryable();

            // Dynamic Filtering
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                query = query.Where(s => s.FullName.ToLower().Contains(dto.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(dto.Specialty))
            {
                query = query.Where(s => s.Specialty.ToLower().Contains(dto.Specialty.ToLower().Trim()));
            }

            // retrieve the students with their enrolled courses count
            var students = await query
                 .Select(s => new StudentReadDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Specialty = s.Specialty,
                    EnrolledCoursesCount = s.Enrollments.Count(e => e.Status != "Canceled")
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(long id)
        {
            var student = await _context.Students
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    StudentDetails = new StudentReadDto
                    {
                        Id = s.Id,
                        FullName = s.FullName,
                        Email = s.Email,
                        PhoneNumber = s.PhoneNumber,
                        Specialty = s.Specialty,
                        EnrolledCoursesCount = s.Enrollments.Count(e => e.Status != "Canceled")
                    },
                    // Mod : TotalAmount / PaidAmount on Enrollment (source of truth)
                    EnrolledCourses = s.Enrollments
                        .Where(e => e.Status != "Canceled")
                        .Select(e => new
                        {
                            EnrollmentId = e.Id,
                            CourseId = e.CourseId,
                            CourseTitle = e.Course.Title,
                            CourseFee = e.TotalAmount,
                            PaidAmount = e.PaidAmount,
                            RemainingAmount = e.TotalAmount - e.PaidAmount,
                            IsFullyPaid = e.PaidAmount >= e.TotalAmount,
                            EnrollmentDate = e.EnrollmentDate,
                            Status = e.Status
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new { message = $"The Student With ID {id} Not Exist." });
            }

            decimal totalRequired = student.EnrolledCourses.Sum(c => c.CourseFee);
            decimal totalPaid = student.EnrolledCourses.Sum(c => c.PaidAmount);
            decimal totalRemaining = totalRequired - totalPaid;

            var response = new
            {
                student.StudentDetails,
                FinancialSummary = new
                {
                    TotalRequiredFees = totalRequired,
                    TotalPaidAmount = totalPaid,
                    TotalRemainingBalance = totalRemaining
                },
                student.EnrolledCourses
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Student data is null, pls fill the fields." });
            // Check for email uniqueness
            var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                return BadRequest(new { message = $"A student with the email '{dto.Email}' already exists." });
            }
            var student = new Student
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Specialty = dto.Specialty
            };
            _context.Students.Add(student);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"A student with the email '{dto.Email}' already exists." });
            }

            var resultDto = new StudentReadDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Specialty = student.Specialty,
                EnrolledCoursesCount = 0
            };

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(long id, [FromBody] StudentUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Student data is null, pls fill the fields." });
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = $"The Student With ID {id} Not Exist." });
            }
            // Check for email uniqueness
            var emailExists = await _context.Students.AnyAsync(s => s.Email.ToLower() == dto.Email.ToLower() && s.Id != id);
            if (emailExists)
            {
                return BadRequest(new { message = $"A student with the email '{dto.Email}' already exists." });
            }
            student.FullName = dto.FullName;
            student.Email = dto.Email;
            student.PhoneNumber = dto.PhoneNumber;
            student.Specialty = dto.Specialty;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"A student with the email '{dto.Email}' already exists." });
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = $"Student with ID {id} does not exist." });
            }

            // --  Deletion is allowed as long as no ACTIVE (non-canceled) enrollment remains
            //  - Direct existence check against Enrollments 
            bool hasActiveEnrollments = await _context.Enrollments
                .AnyAsync(e => e.StudentId == id && e.Status != "Canceled");

            if (hasActiveEnrollments)
            {
                return BadRequest(new { message = "Cannot delete student with active (non-canceled) course enrollments." });
            }

            _context.Students.Remove(student);
            // Any Canceled enrollments (and their payments) are removed automatically via [Cascade].
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Student with ID {id} deleted successfully." });

        }
    }

}
