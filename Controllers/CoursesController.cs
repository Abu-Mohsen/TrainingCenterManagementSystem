using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.DTOs.Course;
using TrainingCenterSystem.Models.Entities;

namespace TrainingCenterSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Select(c => new CourseReadDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    MaxCapacity = c.MaxCapacity,
                    Fee = c.Fee,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "Not Specified Yet.",
                    EnrolledStudentsCount = c.Enrollments.Count
                })
                .ToListAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(long id)
        {
            var course = await _context.Courses
                 .Where(c => c.Id == id)
                 .Select(c => new CourseReadDto
                 {
                     Id = c.Id,
                     Title = c.Title,
                     Description = c.Description,
                     MaxCapacity = c.MaxCapacity,
                     Fee = c.Fee,
                     StartDate = c.StartDate,
                     EndDate = c.EndDate,
                     InstructorId = c.InstructorId,
                     InstructorName = c.Instructor != null ? c.Instructor.FullName : "Not Specified Yet.",
                     EnrolledStudentsCount = c.Enrollments.Count
                 })
                 .FirstOrDefaultAsync();

            if (course == null) return NotFound(new { message = $"The Course With ID {id} Does Not Exist." });

            return Ok(course);

        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Course data is required." });

            if (dto.EndDate <= dto.StartDate) return BadRequest(new { message = "End Date must be after Start Date." });

            if (dto.InstructorId.HasValue)
            {
                var instructorExists = await _context.Instructors.AnyAsync(i => i.Id == dto.InstructorId.Value);
                if (!instructorExists) return BadRequest(new { message = $"Instructor with ID {dto.InstructorId.Value} does not exist." });
            }

            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                MaxCapacity = dto.MaxCapacity,
                Fee = dto.Fee,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                InstructorId = dto.InstructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Fetch The Instractor Name After Saving The Course if InstructorId is provided
            string? instructorName = null;
            if (course.InstructorId.HasValue)
            {
                var instructor = await _context.Instructors.FindAsync(course.InstructorId.Value);
                instructorName = instructor?.FullName;
            }

            var resultDto = new CourseReadDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                MaxCapacity = course.MaxCapacity,
                Fee = course.Fee,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                InstructorId = course.InstructorId,
                InstructorName = instructorName ?? "Not Specified Yet.",
                EnrolledStudentsCount = 0
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(long id, [FromBody] CourseUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Course data is required." });
            if (dto.EndDate <= dto.StartDate) return BadRequest(new { message = "End Date must be after Start Date." });
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { message = $"The Course With ID {id} Does Not Exist." });
            if (dto.InstructorId.HasValue)
            {
                var instructorExists = await _context.Instructors.AnyAsync(i => i.Id == dto.InstructorId.Value);
                if (!instructorExists) return BadRequest(new { message = $"Instructor with ID {dto.InstructorId.Value} does not exist." });
            }
            course.Title = dto.Title;
            course.Description = dto.Description;
            course.MaxCapacity = dto.MaxCapacity;
            course.Fee = dto.Fee;
            course.StartDate = dto.StartDate;
            course.EndDate = dto.EndDate;
            course.InstructorId = dto.InstructorId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(long id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound(new { message = $"The Course With ID {id} Does Not Exist." });

            // Validation : Check if there are any enrollments for this course
            if (course.Enrollments.Any())
            {
                return BadRequest(new { message = "Cannot delete the course because there are students enrolled in it." });
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"The Course With ID {id} Has Been Deleted Successfully." });

        }



    }
}
