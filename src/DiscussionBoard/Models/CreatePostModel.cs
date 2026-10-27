using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class CreatePostModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(10000)]
        public string Content {get; set;} = string.Empty;
        public int? ParentPostId { get; set; }
    }
}
