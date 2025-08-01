using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.API
{
    public class CommentDto
    {
        [Required]
        public required string Content { get; set; }
    }
}
