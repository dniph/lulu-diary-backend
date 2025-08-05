using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers;

[Route("api/friend-requests")]
[ApiController]
public class FriendRequestsController : ControllerBase
{
    private readonly FriendRequestsRepository _friendRequestsRepository;
    private readonly ProfilesRepository _profilesRepository;
    private readonly ILogger<FriendRequestsController> _logger;

    public FriendRequestsController(
        FriendRequestsRepository friendRequestsRepository,
        ProfilesRepository profilesRepository,
        ILogger<FriendRequestsController> logger)
    {
        _friendRequestsRepository = friendRequestsRepository;
        _profilesRepository = profilesRepository;
        _logger = logger;
    }

    [HttpGet("incoming")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetIncomingRequests()
    {
        try
        {
            int profileId = 1;
            var requests = await _friendRequestsRepository.GetIncomingRequestsAsync(profileId);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incoming friend requests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("outgoing")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetOutgoingRequests()
    {
        try
        {
            int profileId = 1;
            var requests = await _friendRequestsRepository.GetOutgoingRequestsAsync(profileId);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting outgoing friend requests");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("send")]
    public async Task<ActionResult<FriendRequestDto>> CreateFriendRequest(FriendRequestDto friendRequest)
    {
        try
        {
            int profileId = 1; // This should be replaced with the actual profile ID from the authenticated user context

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
    public async Task<IActionResult> RejectFriendRequest(int requestId)
    {
        try
        {
            int profileId = 1; // This should be replaced with the actual profile ID from the authenticated user context
            
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
    public async Task<IActionResult> AcceptFriendRequest(int requestId)
    {
        try
        {
            int profileId = 1; // This should be replaced with the actual profile ID from the authenticated user context
            
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
