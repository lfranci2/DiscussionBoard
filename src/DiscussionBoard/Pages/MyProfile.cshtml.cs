using System.Security.Claims;
using DiscussionBoard.Data;
using DiscussionBoard.Models;
using DiscussionBoard.Services;
using DiscussionBoard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Pages
{
    [Authorize]
    public class MyProfile : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IUserProfileService _userProfileService;
        [BindProperty]
        public UserProfile Input { get; set; }

        public MyProfile(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _db = db;
            _userProfileService = userProfileService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    return Challenge();

                Input = new UserProfile(user.DisplayName, user.AvatarId);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Input.DisplayName = Input.DisplayName?.Trim();
            if (string.IsNullOrWhiteSpace(Input.DisplayName) || Input.DisplayName.Length < 2 || Input.DisplayName.Length > 60)
            {
                ModelState.AddModelError(
                    "Profile.DisplayName",
                    "Name must be between 2 and 60 characters."
                );
                return Page();
            }
            bool isValidName = await _userProfileService.IsDisplayNameAvailableAsync(Input.DisplayName, user.Id);
            if (!isValidName)
            {
                ModelState.AddModelError(
                    "Profile.DisplayName",
                    "Name is already taken."
                );
                return Page();
            }

            if (!AvatarCatalog.IsValid(Input.AvatarId))
            {
                ModelState.AddModelError(
                    "Profile.AvatarId",
                    "Invalid avatar ID."
                );
                return Page();
            }
            user.DisplayName = Input.DisplayName;
            user.AvatarId = Input.AvatarId;
            var result = await  _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewData["Message"] = "Profile updated successfully.";
                return Page();
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
