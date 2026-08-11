using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.Entities;
using TrainingCenterSystem.Models.DTOs.Instructor;


namespace TrainingCenterSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        // Dependancy Injection (DI)
        private readonly ApplicationDbContext _context;

        public InstructorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {

            var instructors = await _context.Instructors
                .Select(instructor => new InstructorReadDto
                {
                    Id = instructor.Id,
                    FullName = instructor.FullName,
                    Email = instructor.Email,
                    PhoneNumber = instructor.PhoneNumber,
                    Specialization = instructor.Specialization,
                    AssignedCoursesCount = instructor.Courses.Count
                })
                .ToListAsync();
            
            return Ok(instructors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstructor(long id)
        {
            var instructor = await _context.Instructors
                .Where(i => i.Id == id)
                .Select(i => new
                {
                    instructorDetails = new InstructorReadDto
                    {
                        Id = i.Id,
                        FullName = i.FullName,
                        Email = i.Email,
                        PhoneNumber = i.PhoneNumber,
                        Specialization = i.Specialization,
                        AssignedCoursesCount = i.Courses.Count
                    },
                    AssignedCourses = i.Courses.Select(c => new
                    {
                        CourseId = c.Id,
                        Title = c.Title,
                        MaxCapacity = c.MaxCapacity,
                        Fee = c.Fee,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        EnrolledStudentsCount = c.Enrollments.Count(e => e.Status != "Canceled")
                    }).ToList()
                }).FirstOrDefaultAsync();

            if(instructor == null) return NotFound(new { message = $"The Instructor With ID {id} Not Exist." });

            return Ok(instructor);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateInstructor([FromBody] InstructorCreateDto dto)
        {
            if (dto == null) return BadRequest( new { message = "Instructor data is null, pls fill the fields." });

            // Check for email uniqueness
            var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == dto.Email.ToLower());
            if (emailExists)
            {
                return BadRequest(new { message = $"An instructor with the email '{dto.Email}' already exists." });
            }

            var instructor = new Instructor
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Specialization = dto.Specialization
            };

            _context.Instructors.Add(instructor);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"An instructor with the email '{dto.Email}' already exists." });

            }

            var resultDto = new InstructorReadDto
            {
                Id = instructor.Id,
                FullName = instructor.FullName,
                Email = instructor.Email,
                PhoneNumber = instructor.PhoneNumber,
                Specialization = instructor.Specialization,
                AssignedCoursesCount = 0 // Cuz it's a new instructor with no courses yet
            };

            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor( long id, [FromBody] InstructorUpdateDto dto)
        {
            if (dto == null) return BadRequest("Instructor data is null.");
            var instructor = await _context.Instructors.FirstOrDefaultAsync(e => e.Id == id);
            if (instructor == null)
            {
                return NotFound( new { message = $"The Instructor With ID {id} Not Exist." });
            }
            // Check for email uniqueness
            var emailExists = await _context.Instructors.AnyAsync(i => i.Email.ToLower() == dto.Email.ToLower() && i.Id != id);
            if (emailExists)
            {
                return BadRequest(new { message = $"An instructor with the email '{dto.Email}' already exists." });
            }

            instructor.FullName = dto.FullName;
            instructor.Email = dto.Email;
            instructor.PhoneNumber = dto.PhoneNumber;
            instructor.Specialization = dto.Specialization;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = $"An instructor with the email '{dto.Email}' already exists." });
            }

            return NoContent(); // 204 Status Code: Request successful, no body content to return
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(long id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound(new { message = $"The Instructor With ID {id} Not Exist." });
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync(); // Course.InstructorId is set to NULL automatically (SetNull)

            return Ok(new { message = $"The Instructor With ID {id} Deleted Sucssesfuly." });

        }
    }
}
