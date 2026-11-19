using System.Security.Claims;
using DiscussionBoard.Authorization;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Discussions
{
    [Authorize]
    public class ThreadEditModel : PageModel
    {
        [BindProperty]
        public DiscussionEditViewModel Input { get; set; } = new();
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ThreadEditModel> _logger;
        private readonly  IAuthorizationService _authorizationService;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public ThreadEditModel(
            ApplicationDbContext db,
            ILogger<ThreadEditModel> logger,
            IAuthorizationService authorizationService,
            IApplicationAuthorizationService applicationAuthorizationService)
        {
            _db = db;
            _logger = logger;
            _authorizationService = authorizationService;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

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
                _logger.LogWarning("Requested thread at {Id} does not exist.", id);
                return NotFound();
            }
            bool canEdit = await (_applicationAuthorizationService.CanEditAsync(User, thread));
            if (!canEdit)
            {
                _logger.LogWarning("User cannot edit thread {ThreadId}", thread.Id);
                return Forbid();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                thread,
                ApplicationAuthorizationPolicies.IsOwnerOrAdmin);

            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning("User is not authorized to edit this thread {ThreadId}", thread.Id);
                return Forbid();
            }

            Input = new DiscussionEditViewModel
            {
                Title = thread.Title,
                Content = thread.Content
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
                _logger.LogWarning("Requested thread at {Id} does not exist.", id);
                return NotFound();
            }

            bool canEdit = await (_applicationAuthorizationService.CanEditAsync(User, thread));
            if (!canEdit)
            {
                _logger.LogWarning("User cannot edit thread {ThreadId}", thread.Id);
                return Forbid();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                thread,
                ApplicationAuthorizationPolicies.IsOwnerOrAdmin);

            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning("User is not authorized to edit this thread {ThreadId}", thread.Id);
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            thread.Title = Input.Title;
            thread.Content = Input.Content;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Edited Discussion Thread {ThreadId} in forum {ForumId}", thread.Id, thread.ForumId);
            return RedirectToPage("/Discussions/Details", new { threadId = thread.Id });
        }
    }
}
