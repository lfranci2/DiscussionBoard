using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Services
{
    public class DiscussionThreadService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DiscussionThreadService> _logger;

        public DiscussionThreadService(ApplicationDbContext db, ILogger<DiscussionThreadService> logger)
            {
            _db = db;
            _logger = logger;
            }

        public async Task<List<DiscussionThread>> GetAsync(int id)
        {
            try
            {
                var list = await _db.DiscussionThreads
                    .Where(x => x.Id == id)
                    .ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                var list = new List<DiscussionThread>();
                _logger.LogError(ex.ToString());
                return list;
            }
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
                _logger.LogError(ex.ToString());
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
                _logger.LogError(ex.ToString());
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
                _logger.LogError(ex.ToString());
                return null;
            }
        }

    }
}
