namespace UCourses_Back_End.Core.DTOs.RealTimeDTOs
{
    public class MessageDTO
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public string SenderId { get; set; } = null!;

        public string Content { get; set; } = null!;

        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; }
    }
}
