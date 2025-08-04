using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class Follower
    {
        [Key]
        public int Id { get; set; }

        public int ProfileId { get; set; } // The profile who is being followed

        public int FollowerId { get; set; } // The profile who follows

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp when the follow was created
    }
}
