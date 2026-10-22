using Microsoft.AspNetCore.Identity;

namespace DiscussionBoard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
    }
}
