namespace DiscussionBoard.Models
{
    public class DiscussionThread : DiscussionBase
    {
        public Forum Forum { get; set; } = new Forum();
        public int ForumId { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
