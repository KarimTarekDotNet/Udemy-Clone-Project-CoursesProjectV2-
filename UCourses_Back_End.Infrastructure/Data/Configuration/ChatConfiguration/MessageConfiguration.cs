using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCourses_Back_End.Core.Entites.RealTime;
using UCourses_Back_End.Infrastructure.Data.Seeding;

namespace UCourses_Back_End.Infrastructure.Data.Configuration.ChatConfiguration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Seed data - Sample messages
            builder.HasData(
                new Message
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    PublicId = "MESS-22222222",
                    ConversationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    SenderId = SeedData.StudentUserId,
                    Content = "Hello, I have a question about the course material.",
                    IsRead = true,
                    SentAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                    CreatedAt = new DateOnly(2024, 1, 15)
                },
                new Message
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    PublicId = "MESS-33333333",
                    ConversationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    SenderId = SeedData.InstructorUserId,
                    Content = "Sure! What would you like to know?",
                    IsRead = true,
                    SentAt = new DateTime(2024, 1, 15, 10, 35, 0, DateTimeKind.Utc),
                    CreatedAt = new DateOnly(2024, 1, 15)
                },
                new Message
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    PublicId = "MESS-44444444",
                    ConversationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    SenderId = SeedData.StudentUserId,
                    Content = "Can you explain the dependency injection concept in more detail?",
                    IsRead = false,
                    SentAt = new DateTime(2024, 1, 15, 10, 40, 0, DateTimeKind.Utc),
                    CreatedAt = new DateOnly(2024, 1, 15)
                }
            );
        }
    }
}
