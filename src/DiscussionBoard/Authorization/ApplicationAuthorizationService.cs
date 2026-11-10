using System.Security.Claims;
using DiscussionBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Authorization
{
    public class ApplicationAuthorizationService : IApplicationAuthorizationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationAuthorizationService(
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService
            , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _roleManager = roleManager;
        }

        public async Task<bool> CanEditAsync(ClaimsPrincipal principal, DiscussionBase resource)
        {
            var result = await _authorizationService.AuthorizeAsync(
                principal,
                resource,
                "IsOwnerOrAdmin");

            return result.Succeeded;
        }

        public async Task<IdentityResult> AddRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            bool isAdmin = actingUser.IsInRole("Admin");
            if (!isAdmin)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Acting user isn't admin."
                });
            }

            var currentUserId = actingUser.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (currentUserId == targetUserId)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "You cannot give yourself the Admin role."
                });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            if (string.IsNullOrWhiteSpace(roleName) ||
                !await _roleManager.RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role does not exist."
                });
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveRoleAsync(ClaimsPrincipal actingUser, string targetUserId, string roleName)
        {
            bool isAdmin = actingUser.IsInRole("Admin");
            if (!isAdmin)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Acting user isn't admin."
                });
            }

            var user = await _userManager.FindByIdAsync(targetUserId);
            var currentUserId = actingUser.FindFirstValue(
                ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            if (string.IsNullOrWhiteSpace(roleName) ||
                !await _roleManager.RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role does not exist."
                });
            }

            if (currentUserId == targetUserId)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "You cannot remove your own Admin role."
                });
            }

            var admins = await _userManager.GetUsersInRoleAsync(
                ApplicationRoles.Admin);

            if (admins.Count <= 1)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "You cannot remove the last remaining administrator."
                });
            }

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }
    }
}
