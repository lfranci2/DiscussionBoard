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

        public async Task<Post> GetAsync(int id)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null || post.IsDeleted)
            {
                return null;
            }
            else
            {
                return post;
            }
        }

        public async Task UpdateAsync(Post post)
        {
            var existingPost = await _db.Posts
                .FirstOrDefaultAsync(p => p.Id == post.Id && !p.IsDeleted);

            if (existingPost == null)
            {
                return;
            }
            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            await _db.SaveChangesAsync();
        }

        public async Task<List<Post>> GetForThreadAsync(int id)
        {
            return await _db.Posts
                .Where(p => p.DiscussionThreadId == id && !p.IsDeleted)
                .Include(p => p.ApplicationUser)
                .ToListAsync();
        }

        public async Task AddAsync(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
        }
    }
}
