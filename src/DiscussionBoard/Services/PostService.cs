using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _db;
        public PostService(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<List<Post>> GetForThreadAsync(int id)
        {
            return await _db.Posts
                .Where(p => p.DiscussionThreadId == id && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
        }
    }
}
