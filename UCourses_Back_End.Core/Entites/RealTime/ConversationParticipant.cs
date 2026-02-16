namespace UCourses_Back_End.Core.Entites.RealTime
{
    public class ConversationParticipant
    {
        public Guid ConversationId { get; set; }
        public string UserId { get; set; } = null!;

        public Conversation Conversation { get; set; } = null!;
    }
}