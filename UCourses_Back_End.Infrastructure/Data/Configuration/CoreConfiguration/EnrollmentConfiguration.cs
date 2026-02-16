using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.CoreConfiguration
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.Property(c => c.Status)
                .HasConversion<string>();
            builder.HasData(
                new
                {
                    Id = Guid.Parse(SeedData.EnrollmentId1),
                    PublicId = SeedData.EnrollmentPublicId1,
                    CourseId = Guid.Parse(SeedData.CourseId1),
                    StudentId = Guid.Parse(SeedData.StudentEntityId),
                    Status = PaymentStatus.Captured,
                    CreatedAt = SeedData.SeedDate
                },
                new
                {
                    Id = Guid.Parse(SeedData.EnrollmentId2),
                    PublicId = SeedData.EnrollmentPublicId2,
                    CourseId = Guid.Parse(SeedData.CourseId2),
                    StudentId = Guid.Parse(SeedData.StudentEntityId),
                    Status = PaymentStatus.Captured,
                    CreatedAt = SeedData.SeedDate
                }
            );
        }
    }
}
