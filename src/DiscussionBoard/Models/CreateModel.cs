using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscussionBoard.Models
{
    public class CreateModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(10000)]
        public string Content {get; set;} = string.Empty;
    }
}
