using System;
using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.Database
{
    public class Diary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public string Username { get; set; }  

        public bool IsPublic { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
