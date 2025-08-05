using System.ComponentModel.DataAnnotations;

namespace lulu_diary_backend.Models.API;

public class FriendRequestDto
{
    public string RequestedUsername { get; set; } = string.Empty;
}

public class FriendRequestCreateDto
{
    [Required(ErrorMessage = "RequestedUsername is required")]
    public string RequestedUsername { get; set; } = string.Empty;
}

public class FriendRequestUpdateDto
{
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(accepted|rejected)$", ErrorMessage = "Status must be 'accepted' or 'rejected'")]
    public string Status { get; set; } = string.Empty;
}
