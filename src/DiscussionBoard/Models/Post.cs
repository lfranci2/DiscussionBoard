namespace DiscussionBoard.Models
{
    public class Post : DiscussionBase
    {
        public DiscussionThread DiscussionThread { get; set; } = new DiscussionThread();
        public int DiscussionThreadId { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public Post? ParentPost { get; set; }
        public int? ParentPostId { get; set; }
    }
}
