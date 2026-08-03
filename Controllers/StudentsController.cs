using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.Entities;
using TrainingCenterSystem.Models.DTOs.Student;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                query = query.Where(s => s.FullName.ToLower().Contains(dto.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(dto.Specialty))
            {
                query = query.Where(s => s.Specialty.ToLower().Contains(dto.Specialty.ToLower().Trim()));
            }

            var students = await query.Select(s => new StudentReadDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                Specialty = s.Specialty,
                EnrolledCoursesCount = s.Enrollments.Count
            })
             .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(long id)
        {
            var student = await _context.Students
                .Where(s => s.Id == id)
                .Select(s => new StudentReadDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Specialty = s.Specialty,
                    EnrolledCoursesCount = s.Enrollments.Count
                })
                .FirstOrDefaultAsync();
            if (student == null)
            {
                return NotFound(new { message = $"The Student With ID {id} Not Exist." });
            }
            return Ok(student);
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
                Phone = dto.Phone,
                Specialty = dto.Specialty
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var resultDto = new StudentReadDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
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
            student.Phone = dto.Phone;
            student.Specialty = dto.Specialty;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = $"The Student With ID {id} Does Not Exist." });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"The Student With ID {id} Deleted Successfully." });
        } 
    }
         
}
