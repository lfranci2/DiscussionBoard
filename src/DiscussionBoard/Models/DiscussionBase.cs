using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class DiscussionBase : EntityBase
    {
        [MaxLength(100)]
        public string Title  {get; set;} = string.Empty;
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt  {get; set;} = new DateTimeOffset();
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser {get; set;} = new ApplicationUser();
    }
}
