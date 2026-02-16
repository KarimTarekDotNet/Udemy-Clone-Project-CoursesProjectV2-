using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.CoreConfiguration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Status)
                .HasConversion<string>();

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasData(
                new
                {
                    Id = SeedData.PaymentId1,
                    Amount = SeedData.PaymentAmount1,
                    PaymentDate = SeedData.PaymentDate1,
                    PaymentMethod = SeedData.PaymentMethod1,
                    TransactionId = SeedData.PaymentTransactionId1,
                    Status = PaymentStatus.Captured,
                    EnrollmentId = Guid.Parse(SeedData.EnrollmentId1),
                    StudentId = Guid.Parse(SeedData.StudentEntityId)
                },
                new
                {
                    Id = SeedData.PaymentId2,
                    Amount = SeedData.PaymentAmount2,
                    PaymentDate = SeedData.PaymentDate2,
                    PaymentMethod = SeedData.PaymentMethod2,
                    TransactionId = SeedData.PaymentTransactionId2,
                    Status = PaymentStatus.Captured,
                    EnrollmentId = Guid.Parse(SeedData.EnrollmentId2),
                    StudentId = Guid.Parse(SeedData.StudentEntityId)
                }
            );
        }
    }
}
