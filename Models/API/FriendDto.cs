using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.API;

public class FriendDto
{
    public int ProfileAId { get; set; }
    public int ProfileBId { get; set; }
}

public class FriendCreateDto
{
    [Required]
    public int ProfileBId { get; set; }
}
