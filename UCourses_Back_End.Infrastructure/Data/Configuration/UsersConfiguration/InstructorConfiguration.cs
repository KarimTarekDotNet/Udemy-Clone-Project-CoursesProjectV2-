using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.NetworkInformation;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.UsersConfiguration
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            // Configure Status to be stored as string
            builder.Property(c => c.Status)
                .HasConversion<string>();
            builder.HasData(new
            {
                Id = Guid.Parse(SeedData.InstructorEntityId),
                PublicId = SeedData.InstructorPublicId,
                CreatedAt = SeedData.SeedDate,
                EndContract = SeedData.InstructorEndContract,
                IsApproved = SeedData.IsApproved,
                AppUserId = SeedData.InstructorUserId,
                Status = InstructorStatus.Approved
            });
        }
    }
}