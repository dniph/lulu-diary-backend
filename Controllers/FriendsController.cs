using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers;

[Route("api/profiles/{username}/friends")]
[ApiController]
public class FriendsController : ControllerBase
{
    private readonly FriendsRepository _friendsRepository;
    private readonly ProfilesRepository _profilesRepository;
    private readonly ILogger<FriendsController> _logger;

    public FriendsController(FriendsRepository friendsRepository, ProfilesRepository profilesRepository, ILogger<FriendsController> logger)
    {
        _friendsRepository = friendsRepository;
        _profilesRepository = profilesRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProfileDto>>> GetFriends(string username)
    {
        try
        {
            // First, get the profile ID from the username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound($"Profile with username '{username}' not found");
            }

            var friends = await _friendsRepository.GetFriendsByProfileIdAsync(profile.Id);
            var friendDtos = friends.Select(p => new ProfileDto
            {
                Username = p.Username,
                DisplayName = p.DisplayName,
                AvatarUrl = p.AvatarUrl,
                DiaryVisibility = p.DiaryVisibility
            }).ToList();

            return Ok(friendDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends for username {Username}", username);
            return StatusCode(500, "Internal server error");
        }
    }
}
