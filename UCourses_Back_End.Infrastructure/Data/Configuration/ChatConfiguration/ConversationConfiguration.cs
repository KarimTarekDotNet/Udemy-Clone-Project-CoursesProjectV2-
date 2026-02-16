using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.RealTime;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.ChatConfiguration
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            // Seed data - Sample conversation between instructor and student
            builder.HasData(
                new Conversation
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    PublicId = "CONV-11111111",
                    CreatedAt = SeedData.SeedDate
                }
            );
        }
    }
}
