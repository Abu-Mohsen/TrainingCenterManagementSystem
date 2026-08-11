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
                .Select(e => new EnrollmentReadDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    TotalAmount = e.TotalAmount,
                    PaidAmount = e.PaidAmount,
                    IsFullyPaid = e.PaidAmount >= e.TotalAmount
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(long id)
        {
            var enrollment = await _context.Enrollments
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
                    TotalAmount = e.TotalAmount,
                    PaidAmount = e.PaidAmount,
                    IsFullyPaid = e.PaidAmount >= e.TotalAmount
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

            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null) return NotFound(new { message = $"The Student With ID {dto.StudentId} Does Not Exist." });

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null) return NotFound(new { message = $"The Course With ID {dto.CourseId} Does Not Exist." });

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);
            if (alreadyEnrolled)
                return BadRequest(new { message = $"The Student With ID {dto.StudentId} Is Already Enrolled In The Course With ID {dto.CourseId}." });

            // Mod | Note For Me : Direct COUNT query instead of loading the full Enrollments collection just to count it.
            var activeEnrollmentsCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == dto.CourseId && e.Status != "Canceled");

            if (activeEnrollmentsCount >= course.MaxCapacity)
                return BadRequest(new { message = $"Cannot enroll. The course has reached its maximum capacity of {course.MaxCapacity} students." });

            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                TotalAmount = course.Fee, // snapshot of the fee at enrollment time - never changed afterwards
                PaidAmount = 0,
                Status = "Active"
            };

            _context.Enrollments.Add(enrollment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                //Notes
                // Safety net for the (StudentId, CourseId) unique index, in case two requests
                // race past the AnyAsync check above at the same time.
                return Conflict(new { message = $"The Student With ID {dto.StudentId} Is Already Enrolled In The Course With ID {dto.CourseId}." });
            }

            var resultDto = new EnrollmentReadDto
            {
                Id = enrollment.Id,
                StudentId = student.Id,
                StudentName = student.FullName,
                CourseId = course.Id,
                CourseTitle = course.Title,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                TotalAmount = enrollment.TotalAmount,
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

            if (enrollment.Status == "Canceled")
            {
                return BadRequest(new { message = "This enrollment is already canceled." });
            }

            enrollment.Status = "Canceled";
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Enrollment ID {id} has been canceled successfully." });
        }
    }
}
