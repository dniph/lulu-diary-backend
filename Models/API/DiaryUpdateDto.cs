namespace lulu_diary_backend.Models.API
{
    public class DiaryUpdateDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Visibility { get; set; } // One of: "public", "friends-only", "private"
    }
}
