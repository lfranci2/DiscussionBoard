using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Models
{
    public class CreateModel : PageModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content {get; set;} = string.Empty;
    }
}
