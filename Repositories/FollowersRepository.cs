using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class FollowersRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FollowersRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowersRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public FollowersRepository(AppDbContext context, ILogger<FollowersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all followers for a specific profile.
        /// </summary>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>List of followers for the profile.</returns>
        public async Task<IList<Follower>> GetFollowersAsync(int profileId)
        {
            return await _context.Followers
                .Where(f => f.ProfileId == profileId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all profiles that a specific profile is following.
        /// </summary>
        /// <param name="followerId">Follower ID.</param>
        /// <returns>List of profiles being followed.</returns>
        public async Task<IList<Follower>> GetFollowingAsync(int followerId)
        {
            return await _context.Followers
                .Where(f => f.FollowerId == followerId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new follow relationship.
        /// </summary>
        /// <param name="profileId">Profile ID being followed.</param>
        /// <param name="followerId">Follower ID.</param>
        /// <returns>The created follow relationship.</returns>
        public async Task<Follower> FollowProfileAsync(int profileId, int followerId)
        {
            // Validate that profiles cannot follow themselves
            if (profileId == followerId)
            {
                throw new ArgumentException("Profiles cannot follow themselves.");
            }

            // Check if follow relationship already exists
            var existingFollow = await _context.Followers
                .FirstOrDefaultAsync(f => f.ProfileId == profileId && f.FollowerId == followerId);

            if (existingFollow != null)
            {
                _logger.LogWarning("Follow relationship already exists between follower {FollowerId} and profile {ProfileId}.", followerId, profileId);
                return existingFollow; // Return existing relationship (idempotent)
            }

            var follower = new Follower()
            {
                ProfileId = profileId,
                FollowerId = followerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Followers.AddAsync(follower);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Follow relationship created: follower {FollowerId} now follows profile {ProfileId}.", followerId, profileId);
            return follower;
        }

        /// <summary>
        /// Removes a follow relationship.
        /// </summary>
        /// <param name="profileId">Profile ID being unfollowed.</param>
        /// <param name="followerId">Follower ID.</param>
        /// <returns>The removed follow relationship if found, otherwise null.</returns>
        public async Task<Follower?> UnfollowProfileAsync(int profileId, int followerId)
        {
            var follower = await _context.Followers
                .FirstOrDefaultAsync(f => f.ProfileId == profileId && f.FollowerId == followerId);

            if (follower != null)
            {
                _context.Followers.Remove(follower);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Follow relationship removed: follower {FollowerId} unfollowed profile {ProfileId}.", followerId, profileId);
                return follower;
            }

            _logger.LogWarning("Unfollow failed: no follow relationship found between follower {FollowerId} and profile {ProfileId}.", followerId, profileId);
            return null;
        }
    }
}
