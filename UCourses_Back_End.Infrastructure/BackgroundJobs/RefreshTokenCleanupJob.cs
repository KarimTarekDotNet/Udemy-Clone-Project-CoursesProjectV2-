using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.Interfaces.IBackground;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.BackgroundJobs
{
    public class RefreshTokenCleanupJob : IRefreshTokenCleanupJob
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenCleanupJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var inactiveTokens = tokens.Where(t => !t.IsActive).ToList();
            if (inactiveTokens.Count >= 9)
            {
                _context.RefreshTokens.RemoveRange(inactiveTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ExecuteAllUsers()
        {
            var grouped = await _context.RefreshTokens
                .GroupBy(t => t.UserId)
                .ToListAsync();

            foreach (var group in grouped)
            {
                var inactiveTokens = group.Where(t => !t.IsActive).ToList();
                if (inactiveTokens.Count >= 9)
                {
                    _context.RefreshTokens.RemoveRange(inactiveTokens);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
