using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCourses_Back_End.Core.DTOs.RealTimeDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;

namespace UCourses_Back_End.Api.Controllers.UsersController
{
    [Authorize]
    public class ChatController : BaseController
    {
        public ChatController(IUnitOfWork work) : base(work)
        {
        }

        // ========================= Create Conversation =========================
        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            var conversationId = await work.ChatService.CreateConversationAsync(
                currentUserId, 
                request.ParticipantUserId);

            return Ok(new
            {
                succeeded = true,
                conversationId,
                message = "Conversation created successfully"
            });
        }

        // ========================= Get User Conversations =========================
        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations()
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            var conversations = await work.ChatService.GetUserConversationsAsync(currentUserId);

            return Ok(new
            {
                succeeded = true,
                data = conversations
            });
        }

        // ========================= Send Message =========================
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            var message = await work.ChatService.SendMessageAsync(
                request.ConversationId,
                currentUserId,
                request.Content);

            return Ok(new
            {
                succeeded = true,
                data = message,
                message = "Message sent successfully"
            });
        }

        // ========================= Get Messages =========================
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(
            Guid conversationId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            if (pageSize > 100)
                pageSize = 100;

            var messages = await work.ChatService.GetMessagesAsync(
                conversationId,
                currentUserId,
                pageNumber,
                pageSize);

            return Ok(messages);
        }

        // ========================= Mark Messages as Read =========================
        [HttpPatch("conversations/{conversationId}/mark-read")]
        public async Task<IActionResult> MarkAsRead(Guid conversationId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            await work.ChatService.MarkAsReadAsync(conversationId, currentUserId);

            return Ok(new { message = "Messages marked as read" });
        }

        // ========================= Get Unread Message Count =========================
        [HttpGet("conversations/{conversationId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(Guid conversationId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == null)
                return Unauthorized(new { message = "User ID not found in token" });

            var count = await work.ChatService.GetUnreadMessageCountAsync(conversationId, currentUserId);

            return Ok(new
            {
                conversationId,
                unreadCount = count
            });
        }
    }
}
