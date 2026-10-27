using System.Security.Claims;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages.Discussions
{
    public class Create : PageModel
    {
        [BindProperty]
        public CreateModel Input { get; set; } = new();
        private readonly ApplicationDbContext _db;
        private readonly ILogger<Create> _logger;

        public Create(ApplicationDbContext db,  ILogger<Create> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid with title {Title} and content {Content}",
                    Input.Title,
                    Input.Content);
                return Page();
            }
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Challenge();
            }

            var thread = new DiscussionThread
            {
                Title = Input.Title,
                Content = Input.Content,
                ApplicationUserId = userId
            };
            _db.Add(thread);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Discussions/Details", new { threadId = thread.Id });
        }
    }
}
