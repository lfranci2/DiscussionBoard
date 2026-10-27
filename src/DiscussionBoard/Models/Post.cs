namespace DiscussionBoard.Models
{
    public class Post : DiscussionBase
    {
        public DiscussionThread? DiscussionThread { get; set; }
        public int DiscussionThreadId { get; set; }
        public ICollection<Post> Replies { get; set; } = new List<Post>();
        public Post? ParentPost { get; set; }
        public int? ParentPostId { get; set; }
    }
}
