using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace lulu_diary_backend.Repositories;

public class FriendRequestsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<FriendRequestsRepository> _logger;
    private readonly FriendsRepository _friendsRepository;

    public FriendRequestsRepository(AppDbContext context, ILogger<FriendRequestsRepository> logger, FriendsRepository friendsRepository)
    {
        _context = context;
        _logger = logger;
        _friendsRepository = friendsRepository;
    }

    public async Task<List<FriendRequest>> GetIncomingRequestsAsync(int profileId)
    {
        try
        {
            // TODO: Add authentication middleware check here
            return await _context.FriendRequests
                .Where(fr => fr.RequestedProfileId == profileId && fr.Status == "pending")
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incoming friend requests for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<List<FriendRequest>> GetOutgoingRequestsAsync(int profileId)
    {
        try
        {
            // TODO: Add authentication middleware check here
            return await _context.FriendRequests
                .Where(fr => fr.RequesterProfileId == profileId && fr.Status == "pending")
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting outgoing friend requests for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<FriendRequest?> GetFriendRequestAsync(int id)
    {
        try
        {
            // TODO: Add authentication middleware check here
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friend request {Id}", id);
            throw;
        }
    }

    public async Task<FriendRequest> CreateFriendRequestAsync(int requesterProfileId, Profile requestedProfile)
    {
        try
        {
            // TODO: Add authentication middleware check here
            
            // Validate that requester != requested
            if (requestedProfile.Id == requesterProfileId)
            {
                throw new ArgumentException("Cannot send friend request to oneself");
            }

            // Check if they are already friends
            var areFriends = await _friendsRepository.AreFriendsAsync(requesterProfileId, requestedProfile.Id);
            if (areFriends)
            {
                throw new InvalidOperationException("Users are already friends");
            }

            // Check if there's already a pending request between these users
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => 
                    ((fr.RequesterProfileId == requesterProfileId && fr.RequestedProfileId == requestedProfile.Id) ||
                    (fr.RequesterProfileId == requestedProfile.Id && fr.RequestedProfileId == requesterProfileId)) &&
                    fr.Status == "pending");

            if (existingRequest != null)
            {
                throw new InvalidOperationException("A pending friend request already exists between these users");
            }

            var friendRequest = new FriendRequest
            {
                RequesterProfileId = requesterProfileId,
                RequestedProfileId = requestedProfile.Id,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            return friendRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating friend request from {RequesterProfileId} to {RequestedProfileId}", 
                requesterProfileId, requestedProfile.Id);
            throw;
        }
    }

    public async Task<FriendRequest?> UpdateFriendRequestStatusAsync(int id, string status)
    {
        try
        {
            // TODO: Add authentication middleware check here
            
            var friendRequest = await GetFriendRequestAsync(id);
            if (friendRequest == null)
            {
                return null;
            }

            if (friendRequest.Status != "pending")
            {
                throw new InvalidOperationException("Friend request has already been processed");
            }

            friendRequest.Status = status;

            // If accepted, create friendship
            if (status == "accepted")
            {
                var friendship = new Friend
                {
                    ProfileAId = friendRequest.RequesterProfileId,
                    ProfileBId = friendRequest.RequestedProfileId
                };
                await _friendsRepository.CreateFriendshipAsync(friendship);
            }

            await _context.SaveChangesAsync();

            return await _context.FriendRequests
                .FirstAsync(fr => fr.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating friend request {Id} status to {Status}", id, status);
            throw;
        }
    }
}
