using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.RealTime;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.ChatConfiguration
{
    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            // Seed data - Instructor and Student in conversation
            builder.HasData(
                new ConversationParticipant
                {
                    ConversationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserId = SeedData.InstructorUserId
                },
                new ConversationParticipant
                {
                    ConversationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserId = SeedData.StudentUserId
                }
            );
        }
    }
}
