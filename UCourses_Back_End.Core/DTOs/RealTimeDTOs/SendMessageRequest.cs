using System.ComponentModel.DataAnnotations;

namespace UCourses_Back_End.Core.DTOs.RealTimeDTOs
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "Conversation ID is required")]
        public Guid ConversationId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 2000 characters")]
        public string Content { get; set; } = null!;
    }
}
