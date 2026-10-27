using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages
{
    public class Details : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Forum Forum { get; set; }
        public List<DiscussionThread> Threads { get; private set; }
            = new();

        public Details(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var forum = await _db.Set<Forum>()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (forum == null)
            {
                return NotFound();
            }

            Forum = forum;

            Threads = await _db.Set<DiscussionThread>()
                .Where(t => t.ForumId == id)
                .ToListAsync();

            return Page();
        }

    }
}
