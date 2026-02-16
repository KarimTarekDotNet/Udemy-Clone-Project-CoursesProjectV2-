namespace UCourses_Back_End.Core.Entites.RealTime
{
    public class Message : BaseEntity
    {
        public Message()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("MESS");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        public Guid ConversationId { get; set; }
        public string SenderId { get; set; } = null!;

        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; }

        public Conversation Conversation { get; set; } = null!;
    }
}