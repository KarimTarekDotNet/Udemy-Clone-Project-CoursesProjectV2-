using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.CoreConfiguration
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasData(
                new
                {
                    Id = Guid.Parse(SeedData.SectionId1),
                    PublicId = SeedData.SectionPublicId1,
                    Name = SeedData.SectionName1,
                    Description = SeedData.SectionDescription1,
                    VideoUrl = SeedData.SectionVideoUrl1,
                    PdfUrl = SeedData.SectionPdfUrl1,
                    StartAt = new TimeOnly(10, 0),
                    EndAt = new TimeOnly(12, 0),
                    DayOfWeek = DayOfWeek.Monday,
                    CourseId = Guid.Parse(SeedData.CourseId1),
                    CreatedAt = SeedData.SeedDate
                },
                new
                {
                    Id = Guid.Parse(SeedData.SectionId2),
                    PublicId = SeedData.SectionPublicId2,
                    Name = SeedData.SectionName2,
                    Description = SeedData.SectionDescription2,
                    VideoUrl = SeedData.SectionVideoUrl2,
                    StartAt = new TimeOnly(14, 0),
                    EndAt = new TimeOnly(16, 0),
                    DayOfWeek = DayOfWeek.Tuesday,
                    CourseId = Guid.Parse(SeedData.CourseId2),
                    CreatedAt = SeedData.SeedDate
                }
            );
        }
    }
}
