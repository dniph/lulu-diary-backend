namespace lulu_diary_backend.Models.API
{
    public class ProfileUpdateDto
    {
        public string? Username { get; set; }
        
        public string? DisplayName { get; set; }

        public string? AvatarUrl { get; set; }

        public string? DiaryVisibility { get; set; } // One of: "public", "private"
    }
}
