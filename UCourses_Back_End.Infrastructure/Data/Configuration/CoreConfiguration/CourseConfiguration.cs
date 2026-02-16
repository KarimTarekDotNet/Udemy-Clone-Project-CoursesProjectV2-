using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.CoreConfiguration
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Configure Status to be stored as string
            builder.Property(c => c.Status)
                .HasConversion<string>();

            builder.HasData(
                new
                {
                    Id = Guid.Parse(SeedData.CourseId1),
                    PublicId = SeedData.CoursePublicId1,
                    Name = SeedData.CourseName1,
                    Description = SeedData.CourseDescription1,
                    Price = SeedData.CoursePrice1,
                    ImageUrl = SeedData.CourseImageUrl1,
                    Status = CourseStatus.Published,
                    InstructorId = Guid.Parse(SeedData.InstructorEntityId),
                    DepartmentId = Guid.Parse(SeedData.DepartmentId1),
                    CreatedAt = SeedData.SeedDate
                },
                new
                {
                    Id = Guid.Parse(SeedData.CourseId2),
                    PublicId = SeedData.CoursePublicId2,
                    Name = SeedData.CourseName2,
                    Description = SeedData.CourseDescription2,
                    Price = SeedData.CoursePrice2,
                    ImageUrl = SeedData.CourseImageUrl2,
                    Status = CourseStatus.Draft,
                    InstructorId = Guid.Parse(SeedData.InstructorEntityId),
                    DepartmentId = Guid.Parse(SeedData.DepartmentId2),
                    CreatedAt = SeedData.SeedDate
                }
            );
        }
    }
}
