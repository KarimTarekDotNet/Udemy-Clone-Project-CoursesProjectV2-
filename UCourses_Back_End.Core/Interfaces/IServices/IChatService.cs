using UCourses_Back_End.Core.DTOs.RealTimeDTOs;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IChatService
    {
        Task<Guid> CreateConversationAsync(string user1Id, string user2Id);
        Task<MessageDTO> SendMessageAsync(Guid conversationId, string senderId, string content);
        Task<PagedResult<MessageDTO>> GetMessagesAsync(Guid conversationId, string userId, int pageNumber, int pageSize);
        Task MarkAsReadAsync(Guid conversationId, string userId);
        Task<List<ConversationDTO>> GetUserConversationsAsync(string userId);
        Task<int> GetUnreadMessageCountAsync(Guid conversationId, string userId);
    }
}