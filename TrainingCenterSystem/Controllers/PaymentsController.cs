using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using TrainingCenterSystem.Models.Data;
using TrainingCenterSystem.Models.DTOs.Payment;
using TrainingCenterSystem.Models.Entities;

namespace TrainingCenterSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentReadDto
                {
                    Id = p.Id,
                    EnrollmentId = p.EnrollmentId,
                    StudentName = p.Enrollment.Student.FullName,
                    CourseTitle = p.Enrollment.Course.Title,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes
                })
                .ToListAsync();

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(long id)
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentReadDto
                {
                    Id = p.Id,
                    EnrollmentId = p.EnrollmentId,
                    StudentName = p.Enrollment.Student.FullName,
                    CourseTitle = p.Enrollment.Course.Title,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes
                })
                .FirstOrDefaultAsync();

            if (payment == null)
            {
                return NotFound(new { message = $"Payment with ID {id} does not exist." });
            }

            return Ok(payment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPayment([FromBody] AddPaymentDto paymentDto)
        {
            if (paymentDto == null) return BadRequest(new { message = "Payment data is required." });

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == paymentDto.EnrollmentId);

            if (enrollment == null) return NotFound(new { message = $"Enrollment with ID {paymentDto.EnrollmentId} not found." });

            if (enrollment.Status == "Canceled")
            {
                return BadRequest(new { message = "Cannot add a payment to a canceled enrollment." });
            }

            decimal remainingBalance = enrollment.TotalAmount - enrollment.PaidAmount;

            if (remainingBalance <= 0)
            {
                return BadRequest(new { message = "The enrollment is already fully paid." });
            }

            if (paymentDto.Amount > remainingBalance)
            {
                return BadRequest(new { message = $"The payment amount ({paymentDto.Amount}) exceeds the remaining balance of ({remainingBalance})." });
            }

            // Minimum payment is 30, EXCEPT when the amount exactly settles what's left
             if (paymentDto.Amount < 30 && paymentDto.Amount != remainingBalance)
            {
                return BadRequest(new { message = $"Minimum payment is 30, unless it settles the full remaining balance of ({remainingBalance})." });
            }

            var payment = new Payment
            {
                EnrollmentId = paymentDto.EnrollmentId,
                Amount = paymentDto.Amount,
                Notes = paymentDto.Notes,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            enrollment.PaidAmount += paymentDto.Amount;
           
            if (enrollment.PaidAmount >= enrollment.TotalAmount)
            {
                enrollment.Status = "Completed";
            }

            await _context.SaveChangesAsync();

            var resultDto = new PaymentReadDto
            {
                Id = payment.Id,
                EnrollmentId = payment.EnrollmentId,
                StudentName = enrollment.Student.FullName,
                CourseTitle = enrollment.Course.Title,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                Notes = payment.Notes
            };

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, resultDto);

        }
    }
}
