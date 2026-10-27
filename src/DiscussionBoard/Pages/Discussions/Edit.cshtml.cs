using System.Security.Claims;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Discussions
{
    public class Edit : PageModel
    {
        [BindProperty]
        public CreateModel Input { get; set; } = new();
        private readonly ApplicationDbContext _db;
        private readonly ILogger<Create> _logger;

        public Edit(ApplicationDbContext db,  ILogger<Create> logger)
        {
            _db = db;
            _logger = logger;
        }
        public void OnGet()
        {

        }

        [BindProperty]
        public DiscussionThread Thread { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Challenge();
            }

            var thread = await _db.DiscussionThreads
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thread == null)
            {
                _logger.LogWarning("Requested thread not found");
                return NotFound();
            }

            if (thread.ApplicationUserId != userId)
            {
                _logger.LogWarning("UserId does not match thread");
                return Forbid();
            }

            Thread = new DiscussionThread
            {
                Title = thread.Title,
                Content = thread.Content
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Challenge();
            }

            var thread = await _db.DiscussionThreads
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thread == null)
            {
                _logger.LogWarning("Requested thread not found");
                return NotFound();
            }

            if (thread.ApplicationUserId != userId)
            {
                _logger.LogWarning("UserId does not match thread");
                return Forbid();
            }

            thread.Title = Input.Title;
            thread.Content = Input.Content;

            await _db.SaveChangesAsync();

            return RedirectToPage("./Discussion", new { id = thread.Id });
        }
    }
}
