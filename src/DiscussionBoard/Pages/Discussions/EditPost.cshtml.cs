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
    public class EditPostModel : PageModel
    {
        [BindProperty]
        public DiscussionEditViewModel Input { get; set; } = new();
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EditPostModel> _logger;
        private readonly  IAuthorizationService _authorizationService;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public EditPostModel(
            ApplicationDbContext db,
            ILogger<EditPostModel> logger,
            IAuthorizationService authorizationService,
            IApplicationAuthorizationService applicationAuthorizationService)
        {
            _db = db;
            _logger = logger;
            _authorizationService = authorizationService;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<IActionResult> OnGetAsync(int threadId, int postId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Challenge();
            }

            var post = await _db.Posts
                .FirstOrDefaultAsync(t => t.Id == postId);
            if (post == null)
            {
                _logger.LogWarning("Requested post not found");
                return NotFound();
            }
            bool isCorrectPost = postId == post.Id && threadId == post.DiscussionThreadId;
            if (!isCorrectPost)
            {
                _logger.LogWarning("Requested post not found");
                return NotFound();
            }
            bool canEdit = await (_applicationAuthorizationService.CanEditAsync(User, post));
            if (!canEdit)
            {
                _logger.LogWarning("User cannot edit this post");
                return Forbid();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                post,
                ApplicationAuthorizationPolicies.IsOwnerOrAdmin);

            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning("User is not authorized to edit this post");
                return Forbid();
            }

            Input = new DiscussionEditViewModel
            {
                Title = post.Title,
                Content = post.Content
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int threadId, int postId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Challenge();
            }

            var post = await _db.Posts
                .FirstOrDefaultAsync(t => t.Id == postId);
            if (post == null)
            {
                _logger.LogWarning("Requested post not found");
                return NotFound();
            }
            bool isCorrectPost = postId == post.Id && threadId == post.DiscussionThreadId;
            if (!isCorrectPost)
            {
                _logger.LogWarning("Requested post not found");
                return NotFound();
            }

            bool canEdit = await (_applicationAuthorizationService.CanEditAsync(User, post));
            if (!canEdit)
            {
                _logger.LogWarning("User cannot edit this thread");
                return Forbid();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                post,
                ApplicationAuthorizationPolicies.IsOwnerOrAdmin);

            if (!authorizationResult.Succeeded)
            {
                _logger.LogWarning("User is not authorized to edit this thread");
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                Input = new DiscussionEditViewModel
                {
                    Title = Input.Title,
                    Content = Input.Content
                };

                return Page();
            }

            post.Title = Input.Title;
            post.Content = Input.Content;

            await _db.SaveChangesAsync();

            return RedirectToPage("/Discussions/Details", new { threadId = post.DiscussionThreadId });
        }
    }
}
