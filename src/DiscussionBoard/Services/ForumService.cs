using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace DiscussionBoard.Services
{
    public class ForumService
    {
        private readonly ApplicationDbContext _db;
        public ForumService(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<List<Forum>> GetAllAsync()
        {
            return await _db.Forums
                .Where(f => !f.IsDeleted)
                .ToListAsync();
        }
    }
}
