using Microsoft.AspNetCore.SignalR;

namespace UCourses_Back_End.Infrastructure.RealTime
{
    public class NotificationHub : Hub<INotificationHub>
    {
        private static readonly Dictionary<string, HashSet<string>> userConnections = new (); // to storage all connections

        public override async Task OnConnectedAsync() // run auto if user connected
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                if (userConnections.ContainsKey(userId))
                    userConnections[userId] = new HashSet<string>();

                userConnections[userId].Add(Context.ConnectionId);

                // Notify user they're connected
                await Clients.Caller.ReceiveNotification("Connected to notification hub");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId) && userConnections.ContainsKey(userId))
            {
                userConnections[userId].Remove(Context.ConnectionId);
                if (userConnections[userId].Count == 0)
                    userConnections.Remove(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Join a conversation room for real-time messaging
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Caller.ReceiveNotification($"Joined conversation {conversationId}");
        }

        // Leave a conversation room
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Caller.ReceiveNotification($"Left conversation {conversationId}");
        }

        // Send typing indicator
        public async Task SendTypingIndicator(string conversationId)
        {
            var userId = Context.UserIdentifier;
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .ReceiveTypingIndicator(userId ?? "Unknown", conversationId);
        }

        public static IEnumerable<string> GetAllConnectedUsers() => userConnections.Keys;
        
        public static bool IsUserOnline(string userId) => userConnections.ContainsKey(userId);
    }

    public interface INotificationHub
    {
        Task ReceiveNotification(string message);
        Task ReceiveMessage(object message);
        Task ReceiveTypingIndicator(string userId, string conversationId);
        Task MessageRead(string conversationId, string messageId);
    }
}
