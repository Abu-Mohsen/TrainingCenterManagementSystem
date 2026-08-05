using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.DTOs.Enrollment;
using TrainingCenterSystem.Models.Entities;

namespace TrainingCenterSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Payments)
                .Select(e => new EnrollmentReadDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    TotalAmount = e.Course.Fee,
                    PaidAmount = e.Payments.Sum(p => p.Amount),
                    IsFullyPaid = e.Payments.Sum(p => p.Amount) >= e.Course.Fee
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(long id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Payments)
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentReadDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    TotalAmount = e.Course.Fee,
                    PaidAmount = e.Payments.Sum(p => p.Amount),
                    IsFullyPaid = e.Payments.Sum(p => p.Amount) >= e.Course.Fee
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return NotFound(new { message = $"The Enrollment With ID {id} Does Not Exist." });
            }

            return Ok(enrollment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Enrollment data is null." });

            // cheack if the student exists
            var student = await _context.Students.FindAsync(dto.StudentId);
            if(student == null) return NotFound(new { message = $"The Student With ID {dto.StudentId} Does Not Exist." });

            // check if the course exists
            var course = await _context.Courses.Include(c => c.Enrollments).FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if(course == null) return NotFound(new { message = $"The Course With ID {dto.CourseId} Does Not Exist." });

            // check if the student is already enrolled in the course / [Restriction] A student can only enroll in a course once
            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);
            if (alreadyEnrolled) return BadRequest(new { message = $"The Student With ID {dto.StudentId} Is Already Enrolled In The Course With ID {dto.CourseId}." });

            // check if the course has reached its maximum capacity / [Restriction] A course can only have a limited number of students enrolled
            if(course.Enrollments.Count >= course.MaxCapacity) return BadRequest(new { message = $"Cannot enroll. The course has reached its maximum capacity of {course.MaxCapacity} students." });
            
            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                TotalAmount = course.Fee,

            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var resultDto = new EnrollmentReadDto
            {
                Id = enrollment.Id,
                StudentId = student.Id,
                StudentName = student.FullName,
                CourseId = course.Id,
                CourseTitle = course.Title,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                TotalAmount = course.Fee,
                PaidAmount = 0,
                IsFullyPaid = false
            };

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, resultDto);

        }

        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> DeleteEnrollment(long id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound(new { message = $"The Enrollment With ID {id} Does Not Exist." });
            }

            enrollment.Status = "Canceled";
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Enrollment ID {id} has been canceled successfully." });
        }
    }
}
