using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int ProfileId { get; set; } // Owner of the diary (foreign key to Profiles)

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Required]
        public string Visibility { get; set; } = "public"; // One of: "public", "friends-only", "private"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
