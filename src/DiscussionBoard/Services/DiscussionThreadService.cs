using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class DiscussionThreadService
    {
        private readonly ApplicationDbContext _db;

        public DiscussionThreadService(ApplicationDbContext db)
            {
            _db = db;
            }

        public async Task<DiscussionThread> GetAsync(int id)
        {
            var thread = await _db.DiscussionThreads
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            return thread;
        }

        public async Task<List<DiscussionThread>> GetForForumAsync(int forumId)
        {
            try
            {
                var list = await _db.DiscussionThreads
                    .Where(x => x.ForumId == forumId)
                    .ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                var list = new List<DiscussionThread>();
                return list;
            }
        }

        public async Task<DiscussionThread> AddAsync(DiscussionThread thread)
        {
            try
            {
                var entry = await _db.DiscussionThreads.AddAsync(thread);
                await _db.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DiscussionThread> UpdateAsync(DiscussionThread thread)
        {
            try
            {
                var entry = _db.DiscussionThreads.Update(thread);
                await _db.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
