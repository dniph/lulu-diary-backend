using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; } // Unique identifier (user id)

        [Required]
        public required string Username { get; set; } // Unique username

        public string? DisplayName { get; set; } // Public display name

        public string? AvatarUrl { get; set; } // Optional profile picture URL

        public DateTime CreatedAt { get; set; } // Account creation timestamp (UTC)

        public int FriendsCount { get; set; } // Number of accepted friends

        public int FollowersCount { get; set; } // Number of followers

        public int FollowingCount { get; set; } // Number of accounts the profile is following

        [Required]
        public required string DiaryVisibility { get; set; } // One of: "public", "private"
    }
}
