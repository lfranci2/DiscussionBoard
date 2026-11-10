using DiscussionBoard.Authorization;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Admin
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationAuthorizationService _authorizationService;
        public IEnumerable<SelectListItem> UserList { get; set; }
            = new List<SelectListItem>();
        public bool IsAdmin { get; set; }
        [BindProperty]
        public string? SelectedUserId { get; set; }

        public ApplicationUser? SelectedUser { get; set; }

        [BindProperty]
        public string? UpdatedUserId { get; set; }

        [BindProperty]
        public string? UpdatedRole { get; set; }

        public UsersModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IApplicationAuthorizationService  authorizationService)
        {
            _db = db;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> OnGetAsync(string? selectedUserId)
        {
            await PopulateUserListAsync();

            if (!string.IsNullOrWhiteSpace(selectedUserId))
            {
                var user = await _userManager.FindByIdAsync(selectedUserId);

                if (user != null)
                {
                    await LoadUserAsync(user);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateUserListAsync();
            if (string.IsNullOrWhiteSpace(SelectedUserId))
            {
                ModelState.AddModelError("Users.SelectedUserId", "Please select a user.");
                return Page();
            }

            var user = await _userManager.FindByIdAsync(SelectedUserId);
            if (user == null)
            {
                ModelState.AddModelError("Users.SelectedUserId", "User not found.");
                return Page();
            }

            await LoadUserAsync(user);
            return Page();
        }

        private async Task PopulateUserListAsync()
        {
            UserList = await _db.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = string.Concat(u.DisplayName," | ", u.Email)
                })
                .ToListAsync();
        }

        private async Task LoadUserAsync(ApplicationUser user)
        {
            SelectedUser = user;
            SelectedUserId = user.Id;

            IsAdmin = await _userManager.IsInRoleAsync(
                user, ApplicationRoles.Admin);
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedUserId))
                return BadRequest();

            var result = await _authorizationService.AddRoleAsync(
                User,
                SelectedUserId,
                ApplicationRoles.Admin);

            if (result.Succeeded)
            {
                ViewData["Success"] = "{user.DisplayName} was removed from the Admin role.";
            }
            else
            {
                ViewData["Success"] = string.Join(" ",
                    result.Errors.Select(e => e.Description));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedUserId))
                return BadRequest();

            var result = await _authorizationService.RemoveRoleAsync(
                User,
                SelectedUserId,
                ApplicationRoles.Admin);

            if (result.Succeeded)
            {
                ViewData["Success"] = "{user.DisplayName} was removed from the Admin role.";
            }
            else
            {
                ViewData["Success"] = string.Join(" ",
                    result.Errors.Select(e => e.Description));
            }

            return Page();
        }
    }
}
