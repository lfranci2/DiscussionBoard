using DiscussionBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _db;
        public UserProfileService(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<bool> IsDisplayNameAvailableAsync(string displayName, string? excludeUserId = null)
        {
            var username = displayName.Trim().ToUpper();
            if (username.Length < 2 || username.Length > 60)
            {
                return false;
            }
            bool usernameTaken;
            if (excludeUserId != null)
            {
                usernameTaken = await _db.Users
                .AnyAsync(p => p.DisplayName.ToUpper() == username.ToUpper() && p.Id != excludeUserId);
            }
            else
            {
                usernameTaken = await _db.Users
                    .AnyAsync(p => p.DisplayName.ToUpper() == username);
            }
            return !usernameTaken;
        }
    }
}
