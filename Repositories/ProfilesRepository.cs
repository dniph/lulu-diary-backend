using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class ProfilesRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfilesRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public ProfilesRepository(AppDbContext context, ILogger<ProfilesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all profiles from the database.
        /// </summary>
        /// <returns>List of profiles.</returns>
        public async Task<IList<Profile>> GetProfilesAsync()
        {
            return await _context.Profiles.OrderBy(p => p.Username).ToListAsync();
        }

        /// <summary>
        /// Gets a profile by username.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>Profile if found, otherwise null.</returns>
        public async Task<Profile?> GetProfileByUsernameAsync(string username)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Username == username);
            if (profile == null)
            {
                _logger.LogWarning("Profile with username {Username} not found.", username);
            }
            return profile;
        }

        /// <summary>
        /// Gets a profile by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>Profile if found, otherwise null.</returns>
        public async Task<Profile?> GetProfileByUserIdAsync(string userId)
        {
            return await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        /// <summary>
        /// Updates an existing profile in the database.
        /// Authorization should be handled by middleware before reaching this method.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="dto">Profile update data transfer object.</param>
        /// <returns>Updated profile if found, otherwise null.</returns>
        public async Task<Profile?> UpdateProfileAsync(string username, ProfileUpdateDto dto)
        {
            var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Username == username);
            if (existingProfile == null)
            {
                _logger.LogWarning("Profile update failed: profile with username {Username} not found.", username);
                return null;
            }

            // Update only the provided fields
            if (dto.DisplayName != null)
            {
                existingProfile.DisplayName = dto.DisplayName;
            }

            if (dto.AvatarUrl != null)
            {
                existingProfile.AvatarUrl = dto.AvatarUrl;
            }

            if (dto.DiaryVisibility != null)
            {
                existingProfile.DiaryVisibility = dto.DiaryVisibility;
            }

            await _context.SaveChangesAsync();

            return existingProfile;
        }
    }
}
