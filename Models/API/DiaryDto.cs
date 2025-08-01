using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.API
{
    public class DiaryDto
    {
        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        public required string Visibility { get; set; } // One of: "public", "friends-only", "private"
    }
}
