using System.ComponentModel.DataAnnotations;
using lulu_diary_backend.Attributes;

namespace lulu_diary_backend.Models.API
{
    public class DiaryReactionDto
    {
        [Required]
        [ReactionTypeValidation]
        public required string ReactionType { get; set; } // Must be "like", "love", or "hate"
    }
}
