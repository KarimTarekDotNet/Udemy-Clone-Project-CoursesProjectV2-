using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.UsersConfiguration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasData(new
            {
                Id = Guid.Parse(SeedData.StudentEntityId),
                PublicId = SeedData.StudentPublicId,
                CreatedAt = SeedData.SeedDate,
                AppUserId = SeedData.StudentUserId
            });
        }
    }
}