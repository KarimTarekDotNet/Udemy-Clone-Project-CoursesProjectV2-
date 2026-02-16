using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.Entites.CoreModels
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public Guid EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}