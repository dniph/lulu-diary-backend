using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class DiariesRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DiariesRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiariesRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public DiariesRepository(AppDbContext context, ILogger<DiariesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all diaries for a specific profile.
        /// </summary>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>List of diaries for the profile.</returns>
        public async Task<IList<Diary>> GetDiariesByProfileAsync(int profileId)
        {
            return await _context.Diaries
                .Where(d => d.ProfileId == profileId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Inserts a new diary into the database.
        /// </summary>
        /// <param name="dto">Diary data transfer object.</param>
        /// <param name="profileId">Profile ID of the diary owner.</param>
        /// <returns>The created diary.</returns>
        public async Task<Diary> InsertDiaryAsync(DiaryDto dto, int profileId)
        {
            var diary = new Diary()
            {
                Title = dto.Title,
                Content = dto.Content,
                Visibility = dto.Visibility,
                ProfileId = profileId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Diaries.AddAsync(diary);
            await _context.SaveChangesAsync();
            return diary;
        }

        /// <summary>
        /// Gets a diary by its ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <returns>Diary if found, otherwise null.</returns>
        public async Task<Diary?> GetDiaryAsync(int id)
        {
            var diary = await _context.Diaries.FindAsync(id);
            if (diary == null)
            {
                _logger.LogWarning("Diary with ID {DiaryId} not found.", id);
            }
            return diary;
        }

        /// <summary>
        /// Gets a diary by its ID and profile ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>Diary if found and belongs to profile, otherwise null.</returns>
        public async Task<Diary?> GetDiaryByProfileAsync(int id, int profileId)
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == id && d.ProfileId == profileId);
            
            if (diary == null)
            {
                _logger.LogWarning("Diary with ID {DiaryId} not found for profile {ProfileId}.", id, profileId);
            }
            return diary;
        }

        /// <summary>
        /// Updates an existing diary in the database.
        /// Authorization should be handled by middleware before reaching this method.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <param name="dto">Diary update data transfer object.</param>
        /// <param name="profileId">Profile ID for verification.</param>
        /// <returns>Updated diary if found, otherwise null.</returns>
        public async Task<Diary?> UpdateDiaryAsync(int id, DiaryUpdateDto dto, int profileId)
        {
            var existingDiary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == id && d.ProfileId == profileId);
            
            if (existingDiary == null)
            {
                _logger.LogWarning("Diary update failed: diary with ID {DiaryId} not found for profile {ProfileId}.", id, profileId);
                return null;
            }

            // Update only the provided fields
            if (dto.Title != null)
            {
                existingDiary.Title = dto.Title;
            }

            if (dto.Content != null)
            {
                existingDiary.Content = dto.Content;
            }

            if (dto.Visibility != null)
            {
                existingDiary.Visibility = dto.Visibility;
            }

            existingDiary.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existingDiary;
        }

        /// <summary>
        /// Deletes a diary from the database by its ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <param name="profileId">Profile ID for verification.</param>
        /// <returns>Deleted diary if found, otherwise null.</returns>
        public async Task<Diary?> DeleteDiaryAsync(int id, int profileId)
        {
            var diary = await _context.Diaries
                .FirstOrDefaultAsync(d => d.Id == id && d.ProfileId == profileId);
            
            if (diary != null)
            {
                _context.Diaries.Remove(diary);
                await _context.SaveChangesAsync();
                return diary;
            }
            
            _logger.LogWarning("Diary deletion failed: diary with ID {DiaryId} not found for profile {ProfileId}.", id, profileId);
            return null;
        }

        /// <summary>
        /// Gets public diaries for the public feed.
        /// Considers both diary visibility and profile diaryVisibility settings.
        /// </summary>
        /// <param name="limit">Maximum number of diaries to return.</param>
        /// <param name="offset">Number of diaries to skip for pagination.</param>
        /// <returns>List of public diaries ordered by creation date (newest first).</returns>
        public async Task<IList<(Diary Diary, Profile Profile)>> GetPublicDiariesWithProfilesAsync(int limit, int offset)
        {
            return await _context.Diaries
                .Join(_context.Profiles, d => d.ProfileId, p => p.Id, (d, p) => new { Diary = d, Profile = p })
                .Where(dp => 
                    dp.Diary.Visibility == "public" &&           // Diary itself is public
                    dp.Profile.DiaryVisibility == "public"       // Profile allows public diary visibility
                )
                .OrderByDescending(dp => dp.Diary.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Select(dp => new ValueTuple<Diary, Profile>(dp.Diary, dp.Profile))
                .ToListAsync();
        }

        /// <summary>
        /// Gets diaries for the personalized feed of an authenticated user.
        /// Considers both diary visibility and profile diaryVisibility settings.
        /// Includes:
        /// - Public diaries from profiles with public diaryVisibility
        /// - Friends-only diaries from friends (if their diaryVisibility allows)
        /// - Private diaries from the user's own profile (regardless of profile diaryVisibility)
        /// </summary>
        /// <param name="currentProfileId">Current user's profile ID.</param>
        /// <param name="limit">Maximum number of diaries to return.</param>
        /// <param name="offset">Number of diaries to skip for pagination.</param>
        /// <returns>List of diaries for the personalized feed ordered by creation date (newest first).</returns>
        public async Task<IList<(Diary Diary, Profile Profile)>> GetFeedDiariesWithProfilesAsync(int currentProfileId, int limit, int offset)
        {
            // Get friend profile IDs
            var friendships = await _context.Friends
                .Where(f => f.ProfileAId == currentProfileId || f.ProfileBId == currentProfileId)
                .ToListAsync();

            var friendProfileIds = friendships
                .Select(f => f.ProfileAId == currentProfileId ? f.ProfileBId : f.ProfileAId)
                .ToList();

            // Build the query for the feed with profile diaryVisibility consideration
            var query = _context.Diaries
                .Join(_context.Profiles, d => d.ProfileId, p => p.Id, (d, p) => new { Diary = d, Profile = p })
                .Where(dp =>
                    // Public diaries from profiles with public diaryVisibility
                    (dp.Diary.Visibility == "public" && dp.Profile.DiaryVisibility == "public") ||
                    // Friends-only diaries from friends (if their profile allows diary visibility)
                    (dp.Diary.Visibility == "friends-only" && 
                     friendProfileIds.Contains(dp.Diary.ProfileId) && 
                     dp.Profile.DiaryVisibility == "public") ||
                     // Self friend-only diaries
                     (dp.Diary.Visibility == "friends-only" &&
                     dp.Profile.Id == currentProfileId) ||
                    // Private diaries from own profile (always allowed regardless of profile diaryVisibility)
                    (dp.Diary.Visibility == "private" && dp.Diary.ProfileId == currentProfileId)
                );

            return await query
                .OrderByDescending(dp => dp.Diary.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Select(dp => new ValueTuple<Diary, Profile>(dp.Diary, dp.Profile))
                .ToListAsync();
        }

        /// <summary>
        /// Gets the count of public diaries for pagination purposes.
        /// Considers both diary visibility and profile diaryVisibility settings.
        /// </summary>
        /// <returns>Total count of public diaries.</returns>
        public async Task<int> GetPublicDiariesCountAsync()
        {
            return await _context.Diaries
                .Join(_context.Profiles, d => d.ProfileId, p => p.Id, (d, p) => new { Diary = d, Profile = p })
                .Where(dp => 
                    dp.Diary.Visibility == "public" &&           // Diary itself is public
                    dp.Profile.DiaryVisibility == "public"       // Profile allows public diary visibility
                )
                .CountAsync();
        }

        /// <summary>
        /// Gets the count of diaries for a user's personalized feed for pagination purposes.
        /// Considers both diary visibility and profile diaryVisibility settings.
        /// </summary>
        /// <param name="currentProfileId">Current user's profile ID.</param>
        /// <returns>Total count of diaries in the personalized feed.</returns>
        public async Task<int> GetFeedDiariesCountAsync(int currentProfileId)
        {
            // Get friend profile IDs
            var friendships = await _context.Friends
                .Where(f => f.ProfileAId == currentProfileId || f.ProfileBId == currentProfileId)
                .ToListAsync();

            var friendProfileIds = friendships
                .Select(f => f.ProfileAId == currentProfileId ? f.ProfileBId : f.ProfileAId)
                .ToList();

            // Count diaries in the feed with profile diaryVisibility consideration
            return await _context.Diaries
                .Join(_context.Profiles, d => d.ProfileId, p => p.Id, (d, p) => new { Diary = d, Profile = p })
                .Where(dp =>
                    // Public diaries from profiles with public diaryVisibility
                    (dp.Diary.Visibility == "public" && dp.Profile.DiaryVisibility == "public") ||
                    
                    // Friends-only diaries from friends (if their profile allows diary visibility)
                    (dp.Diary.Visibility == "friends-only" && 
                    friendProfileIds.Contains(dp.Diary.ProfileId) && 
                    dp.Profile.DiaryVisibility == "public") ||
                    
                    // Private diaries from own profile (always allowed regardless of profile diaryVisibility)
                    (dp.Diary.Visibility == "private" && dp.Diary.ProfileId == currentProfileId)
                )
                .CountAsync();
        }
    }
}
