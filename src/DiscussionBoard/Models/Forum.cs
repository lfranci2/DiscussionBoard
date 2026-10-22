namespace DiscussionBoard.Models
{
    public class Forum : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
    }
}
