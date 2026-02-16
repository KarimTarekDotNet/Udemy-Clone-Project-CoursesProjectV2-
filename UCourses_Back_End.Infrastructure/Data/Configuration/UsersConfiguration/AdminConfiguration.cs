using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.UsersConfiguration
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasData(new
            {
                Id = Guid.Parse(SeedData.AdminEntityId),
                PublicId = SeedData.AdminPublicId,
                CreatedAt = SeedData.SeedDate,
                AppUserId = SeedData.AdminUserId
            });
        }
    }
}