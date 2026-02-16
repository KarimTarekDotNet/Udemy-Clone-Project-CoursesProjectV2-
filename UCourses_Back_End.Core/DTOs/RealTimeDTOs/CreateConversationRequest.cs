using System.ComponentModel.DataAnnotations;

namespace UCourses_Back_End.Core.DTOs.RealTimeDTOs
{
    public class CreateConversationRequest
    {
        [Required(ErrorMessage = "Participant user ID is required")]
        public string ParticipantUserId { get; set; } = null!;
    }
}
