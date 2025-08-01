using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public int DiaryId { get; set; } // Identifier of the diary this comment belongs to

        public int ProfileId { get; set; } // Identifier of the profile who made the comment

        [Required]
        public required string Content { get; set; } // Text content of the comment

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the comment was created

        public DateTime? UpdatedAt { get; set; } // Timestamp when the comment was last updated
    }
}
