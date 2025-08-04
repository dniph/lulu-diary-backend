using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class CommentReaction
    {
        [Key]
        public int Id { get; set; }

        public int CommentId { get; set; } // Comment being reacted to

        public int DiaryId { get; set; } // Diary that contains the comment

        public int ProfileId { get; set; } // Profile who reacted

        [Required]
        public required string ReactionType { get; set; } // Reaction type: "like", "love", "laugh", "sad", "angry", "surprised"
    }
}
