using System.ComponentModel.DataAnnotations;

namespace DiscussionBoard.Models
{
    public class DiscussionEditViewModel
    {
        [Required]
        [MaxLength(160)]
        [MinLength(3)]
        public string? Title { get; set; }
        [Required]
        [MaxLength(10000)]
        [MinLength(1)]
        public string? Content { get; set; }
    }
}
