using lulu_diary_backend.Context;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace lulu_diary_backend.Repositories;

public class FriendsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<FriendsRepository> _logger;

    public FriendsRepository(AppDbContext context, ILogger<FriendsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Friend>> GetFriendsByProfileIdAsync(int profileId)
    {
        try
        {
            // TODO: Add authentication middleware check here
            return await _context.Friends
                .Where(f => f.ProfileAId == profileId || f.ProfileBId == profileId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<Friend?> GetFriendshipAsync(int profileAId, int profileBId)
    {
        try
        {
            // TODO: Add authentication middleware check here
            return await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.ProfileAId == profileAId && f.ProfileBId == profileBId) ||
                    (f.ProfileAId == profileBId && f.ProfileBId == profileAId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friendship between profiles {ProfileAId} and {ProfileBId}", profileAId, profileBId);
            throw;
        }
    }

    public async Task<Friend> CreateFriendshipAsync(Friend friend)
    {
        try
        {
            // TODO: Add authentication middleware check here
            
            // Validate that profileAId != profileBId
            if (friend.ProfileAId == friend.ProfileBId)
            {
                throw new ArgumentException("Cannot create friendship with oneself");
            }

            // Check if friendship already exists
            var existingFriendship = await GetFriendshipAsync(friend.ProfileAId, friend.ProfileBId);
            if (existingFriendship != null)
            {
                throw new InvalidOperationException("Friendship already exists");
            }

            // Ensure consistent ordering (smaller ID first)
            if (friend.ProfileAId > friend.ProfileBId)
            {
                (friend.ProfileAId, friend.ProfileBId) = (friend.ProfileBId, friend.ProfileAId);
            }

            friend.CreatedAt = DateTime.UtcNow;
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return await _context.Friends
                .FirstAsync(f => f.Id == friend.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating friendship between profiles {ProfileAId} and {ProfileBId}", friend.ProfileAId, friend.ProfileBId);
            throw;
        }
    }

    public async Task<bool> DeleteFriendshipAsync(int profileAId, int profileBId)
    {
        try
        {
            // TODO: Add authentication middleware check here
            
            var friendship = await GetFriendshipAsync(profileAId, profileBId);
            if (friendship == null)
            {
                return false;
            }

            _context.Friends.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting friendship between profiles {ProfileAId} and {ProfileBId}", profileAId, profileBId);
            throw;
        }
    }

    public async Task<bool> AreFriendsAsync(int profileAId, int profileBId)
    {
        try
        {
            var friendship = await GetFriendshipAsync(profileAId, profileBId);
            return friendship != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking friendship between profiles {ProfileAId} and {ProfileBId}", profileAId, profileBId);
            throw;
        }
    }
}
