namespace UCourses_Back_End.Core.DTOs.RealTimeDTOs
{
    public class ConversationDTO
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
        public List<string> ParticipantIds { get; set; } = new();
    }
}
