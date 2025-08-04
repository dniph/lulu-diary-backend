using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class DiaryReaction
    {
        [Key]
        public int Id { get; set; }

        public int DiaryId { get; set; } // Diary being reacted to

        public int ProfileId { get; set; } // Profile who reacted

        [Required]
        public required string ReactionType { get; set; } // Reaction type (e.g., "like", "love", etc.)
    }
}
