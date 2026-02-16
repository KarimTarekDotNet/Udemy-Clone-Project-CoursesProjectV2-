using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.DTOs.RealTimeDTOs;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.RealTime;
using UCourses_Back_End.Core.Exceptions;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.ModelsView;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.RealTime;

namespace UCourses_Back_End.Infrastructure.Services.UserServices
{
    public class ChatService : IChatService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<ChatService> logger;
        private readonly IHubContext<NotificationHub, INotificationHub> hubContext;

        public ChatService(IMapper mapper, ApplicationDbContext context, ILogger<ChatService> logger, UserManager<AppUser> userManager, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
            this.hubContext = hubContext;
        }

        public async Task<Guid> CreateConversationAsync(string user1Id, string user2Id)
        {
            var user1 = await userManager.FindByIdAsync(user1Id);
            if(user1 == null)
            {
                logger.LogWarning("User {UserId} not found when creating conversation", user1Id);
                throw new NotFoundException("User", user1Id);
            }

            var user2 = await userManager.FindByIdAsync(user2Id);
            if(user2 == null)
            {
                logger.LogWarning("User {UserId} not found when creating conversation", user2Id);
                throw new NotFoundException("User", user2Id);
            }

            var existingConversation = await context.Conversations
                .Where(x => x.Participants.Any(x => x.UserId == user1Id))
                .Where(x => x.Participants.Any(x => x.UserId == user2Id))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                logger.LogInformation("Conversation already exists between {User1} and {User2}", user1Id, user2Id);
                return existingConversation.Id;
            }

            var conversation = new Conversation();
            await context.Conversations.AddAsync(conversation);

            await context.ConversationParticipants.AddAsync(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = user1Id
            });
            await context.ConversationParticipants.AddAsync(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = user2Id
            });
            await context.SaveChangesAsync();
            
            logger.LogInformation("Conversation {ConversationId} created between {User1} and {User2}", 
                conversation.Id, user1Id, user2Id);
            
            return conversation.Id;
        }

        public async Task<PagedResult<MessageDTO>> GetMessagesAsync(Guid conversationId, string userId, int pageNumber, int pageSize)
        {
            var isParticipant = await context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                logger.LogWarning("User {UserId} tried to access Conversation {ConversationId} without permission",
                    userId, conversationId);
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            var totalCount = await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .CountAsync();

            var messages = await context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<MessageDTO>(mapper.ConfigurationProvider) 
                .ToListAsync();

            return new PagedResult<MessageDTO>
            {
                Items = messages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task MarkAsReadAsync(Guid conversationId, string userId)
        {
            var isParticipant = await context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                logger.LogWarning("User {UserId} tried to mark messages as read in Conversation {ConversationId} without permission",
                    userId, conversationId);
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            var unreadMessages = await context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsRead && m.SenderId != userId)
                .ToListAsync();

            if (unreadMessages.Count == 0)
                return;

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                
                // Notify sender that their message was read
                await hubContext.Clients.User(message.SenderId)
                    .MessageRead(conversationId.ToString(), message.Id.ToString());
            }

            await context.SaveChangesAsync();

            logger.LogInformation("{Count} messages marked as read in Conversation {ConversationId} by {UserId}",
                unreadMessages.Count, conversationId, userId);
        }

        public async Task<MessageDTO> SendMessageAsync(Guid conversationId, string senderId, string content)
        {
            var isParticipant = await context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == senderId);

            if (!isParticipant)
            {
                logger.LogWarning("User {SenderId} tried to send message in Conversation {ConversationId} without permission",
                    senderId, conversationId);
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            var messageDto = mapper.Map<MessageDTO>(message);

            // Send real-time message to conversation group
            await hubContext.Clients.Group($"conversation_{conversationId}")
                .ReceiveMessage(new
                {
                    messageDto.Id,
                    messageDto.ConversationId,
                    messageDto.SenderId,
                    messageDto.Content,
                    messageDto.SentAt,
                    messageDto.IsRead
                });

            // Send notification to offline participants
            var participants = await context.ConversationParticipants
                .Where(p => p.ConversationId == conversationId && p.UserId != senderId)
                .Select(p => p.UserId)
                .ToListAsync();

            foreach (var participantId in participants)
            {
                // Check if user is online before sending notification
                if (!NotificationHub.IsUserOnline(participantId))
                {
                    await hubContext.Clients.User(participantId)
                        .ReceiveNotification($"New message from {senderId}");
                }
            }

            logger.LogInformation("Message {MessageId} sent in Conversation {ConversationId} by {SenderId}",
                message.Id, conversationId, senderId);

            return messageDto;
        }

        public async Task<List<ConversationDTO>> GetUserConversationsAsync(string userId)
        {
            var conversations = await context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => new ConversationDTO
                {
                    Id = cp.Conversation.Id,
                    PublicId = cp.Conversation.PublicId,
                    CreatedAt = cp.Conversation.CreatedAt,
                    ParticipantIds = cp.Conversation.Participants
                        .Select(p => p.UserId)
                        .ToList()
                })
                .ToListAsync();

            return conversations;
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid conversationId, string userId)
        {
            var isParticipant = await context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                logger.LogWarning("User {UserId} tried to get unread count for Conversation {ConversationId} without permission",
                    userId, conversationId);
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            var count = await context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsRead && m.SenderId != userId)
                .CountAsync();

            return count;
        }
    }
}
