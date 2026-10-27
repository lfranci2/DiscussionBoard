using System.Security.Claims;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Discussions
{
    public class Details : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly PostService _posts;
        private readonly ILogger<Details> _logger;
        [BindProperty]
        public CreatePostModel Input { get; set; } = new();
        public DiscussionThread Thread { get; set; }
        public List<PostTreeBuilder.Node> PostTree { get; private set; }
            = new();

        public Details(ApplicationDbContext db, PostService posts, ILogger<Details> logger)
        {
            _db = db;
            _posts = posts;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int threadId)
        {
            var thread = await _db.DiscussionThreads
                .Include(t => t.Posts)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null)
            {
                return NotFound();
            }

            Thread = thread;
            var posts = await _posts.GetForThreadAsync(threadId);
            PostTree = PostTreeBuilder.Build(posts);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int threadId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Challenge();

            var post = new Post
            {
                Title = Input.Title,
                Content = Input.Content,
                ParentPostId = Input.ParentPostId,
                ApplicationUserId = userId,
                DiscussionThreadId = threadId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Discussions/Details", new { threadId });
        }

        public async Task<IActionResult> OnPostReplyAsync(int threadId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            if (!Input.ParentPostId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(Input.ParentPostId),
                    "A parent post is required for a reply.");

                return await ReloadPageAsync(threadId);
            }

            var parent = await _db.Posts
                .FirstOrDefaultAsync(p =>
                    p.Id == Input.ParentPostId.Value &&
                    p.DiscussionThreadId == threadId);

            if (parent == null)
            {
                return NotFound();
            }

            // A reply doesn't have a title field in its form.
            // Use a placeholder if Post.Title is non-nullable.
            var post = new Post
            {
                Title = string.IsNullOrWhiteSpace(Input.Title)
                    ? "Reply"
                    : Input.Title,
                Content = Input.Content,
                ParentPostId = parent.Id,
                ApplicationUserId = userId,
                DiscussionThreadId = threadId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Created reply {PostId} to post {ParentPostId}",
                post.Id, parent.Id);

            return await ReloadPageAsync(threadId);
        }

        private async Task<IActionResult> ReloadPageAsync(int threadId)
        {
            var thread = await _db.DiscussionThreads
                .Include(t => t.Posts)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null)
            {
                return NotFound();
            }

            Thread = thread;

            var posts = await _posts.GetForThreadAsync(threadId);
            PostTree = PostTreeBuilder.Build(posts);

            return Page();
        }
    }
}
