using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.CoreConfiguration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasData(
                new
                {
                    Id = Guid.Parse(SeedData.DepartmentId1),
                    PublicId = SeedData.DepartmentPublicId1,
                    Name = SeedData.DepartmentName1,
                    Description = SeedData.DepartmentDescription1,
                    ImageUrl = SeedData.DepartmentImageUrl1,
                    CreatedAt = SeedData.SeedDate
                },
                new
                {
                    Id = Guid.Parse(SeedData.DepartmentId2),
                    PublicId = SeedData.DepartmentPublicId2,
                    Name = SeedData.DepartmentName2,
                    Description = SeedData.DepartmentDescription2,
                    ImageUrl = SeedData.DepartmentImageUrl2,
                    CreatedAt = SeedData.SeedDate
                }
            );
        }
    }
}
