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
    }
}
