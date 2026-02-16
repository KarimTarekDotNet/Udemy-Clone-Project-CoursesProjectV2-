namespace UCourses_Back_End.Core.Entites.RealTime
{
    public class Conversation : BaseEntity
    {
        public Conversation()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("CONV");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        public ICollection<ConversationParticipant> Participants { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = null!;
    }
}