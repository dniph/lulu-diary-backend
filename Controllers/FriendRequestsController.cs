using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers;

[Route("api/friend-requests")]
[ApiController]
public class FriendRequestsController : ControllerBase
{
    private readonly FriendRequestsRepository _friendRequestsRepository;
    private readonly ProfilesRepository _profilesRepository;
    private readonly UserContext _userContext;
    private readonly ILogger<FriendRequestsController> _logger;

    public FriendRequestsController(
        FriendRequestsRepository friendRequestsRepository,
        ProfilesRepository profilesRepository,
        UserContext userContext,
        ILogger<FriendRequestsController> logger)
    {
        _friendRequestsRepository = friendRequestsRepository;
        _profilesRepository = profilesRepository;
        _userContext = userContext;
        _logger = logger;
    }

    [HttpGet("incoming")]
    [Authorize]
    public async Task<ActionResult<List<object>>> GetIncomingRequests()
    {
        try
        {
            int profileId = _userContext.CurrentUserProfile.Id;
            var requests = await _friendRequestsRepository.GetIncomingRequestsWithProfileAsync(profileId);
            var result = requests.Select(x => new {
                request = x.Request,
                profile = x.RequesterProfile
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incoming friend requests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("outgoing")]
    [Authorize]
    public async Task<ActionResult<List<object>>> GetOutgoingRequests()
    {
        try
        {
            int profileId = _userContext.CurrentUserProfile.Id;
            var requests = await _friendRequestsRepository.GetOutgoingRequestsWithProfileAsync(profileId);
            var result = requests.Select(x => new {
                request = x.Request,
                profile = x.RequestedProfile
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting outgoing friend requests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("send")]
    [Authorize]
    public async Task<ActionResult<FriendRequestDto>> CreateFriendRequest(FriendRequestDto friendRequest)
    {
        try
        {
            int profileId = _userContext.CurrentUserProfile.Id;

            var requestedProfile = await _profilesRepository.GetProfileByUsernameAsync(friendRequest.RequestedUsername);
            if (requestedProfile == null)
            {
                return NotFound("Requested profile not found");
            }

            var createdRequest = await _friendRequestsRepository.CreateFriendRequestAsync(profileId, requestedProfile);

            return Ok(createdRequest);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating friend request");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("reject/{requestId}")]
    [Authorize]
    public async Task<IActionResult> RejectFriendRequest(int requestId)
    {
        try
        {
            int profileId = _userContext.CurrentUserProfile.Id;
            
            // First, get the friend request to validate the user can reject it
            var friendRequest = await _friendRequestsRepository.GetFriendRequestAsync(requestId);
            if (friendRequest == null)
            {
                return NotFound("Friend request not found");
            }

            // Only the requested profile can reject the request
            if (friendRequest.RequestedProfileId != profileId)
            {
                return Forbid("You can only reject friend requests sent to you");
            }

            var updatedRequest = await _friendRequestsRepository.UpdateFriendRequestStatusAsync(requestId, "rejected");
            if (updatedRequest == null)
            {
                return NotFound("Friend request not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting friend request {RequestId}", requestId);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPost("accept/{requestId}")]
    [Authorize]
    public async Task<IActionResult> AcceptFriendRequest(int requestId)
    {
        try
        {
            int profileId = _userContext.CurrentUserProfile.Id;
            
            // First, get the friend request to validate the user can accept it
            var friendRequest = await _friendRequestsRepository.GetFriendRequestAsync(requestId);
            if (friendRequest == null)
            {
                return NotFound("Friend request not found");
            }

            // Only the requested profile can accept the request
            if (friendRequest.RequestedProfileId != profileId)
            {
                return Forbid("You can only accept friend requests sent to you");
            }

            var updatedRequest = await _friendRequestsRepository.UpdateFriendRequestStatusAsync(requestId, "accepted");
            if (updatedRequest == null)
            {
                return NotFound("Friend request not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting friend request {RequestId}", requestId);
            return StatusCode(500, "Internal server error");
        }
    }
}
