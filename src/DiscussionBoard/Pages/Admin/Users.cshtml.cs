using DiscussionBoard.Authorization;
using DiscussionBoard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiscussionBoard.Pages.Admin
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class Users : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<SelectListItem> UserList { get; set; }
        public int SelectedUserId { get; set; }

        public Users(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            await PopulateUserListAsync();
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

        /*public async Task<IActionResult> OnPostUpdateRoleAsync()
        {

        }*/
    }
}
