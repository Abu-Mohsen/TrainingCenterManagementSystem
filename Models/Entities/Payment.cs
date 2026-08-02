using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenterSystem.Models.Entities
{
    public class Payment
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long EnrollmentId { get; set; }

        [ForeignKey(nameof(EnrollmentId))]
        public Enrollment Enrollment { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(250)]
        public string? Notes { get; set; }
    }
}
