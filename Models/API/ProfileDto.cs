using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.API
{
    public class ProfileDto
    {
        [Required]
        public required string Username { get; set; }

        public string? DisplayName { get; set; }

        public string? AvatarUrl { get; set; }

        public required string DiaryVisibility { get; set; }
    }
}
