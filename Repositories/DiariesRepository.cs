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
        /// Gets all diaries from the database.
        /// </summary>
        /// <returns>List of diaries.</returns>
        public async Task<IList<Diary>> GetDiariesAsync()
        {
            return await _context.Diaries.OrderBy(d => d.Id).ToListAsync();
        }

        /// <summary>
        /// Inserts a new diary into the database.
        /// </summary>
        /// <param name="dto">Diary data transfer object.</param>
        /// <returns>The created diary.</returns>
        public async Task<Diary> InsertDiaryAsync(DiaryDto dto)
        {
            var diary = new Diary()
            {
                Title = dto.Title,
                Content = dto.Content,
                Username = dto.Username,
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
        /// Updates an existing diary in the database.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <param name="dto">Diary update data transfer object.</param>
        /// <returns>Updated diary if found, otherwise null.</returns>
        public async Task<Diary?> UpdateDiaryAsync(int id, DiaryUpdateDto dto)
        {
            var existingDiary = await _context.Diaries.FindAsync(id);
            if (existingDiary == null)
            {
                _logger.LogWarning("Diary update failed: diary with ID {DiaryId} not found.", id);
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

            existingDiary.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return existingDiary;
        }

        /// <summary>
        /// Deletes a diary from the database by its ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <returns>Deleted diary if found, otherwise null.</returns>
        public async Task<Diary?> DeleteDiaryAsync(int id)
        {
            var diary = await _context.Diaries.FindAsync(id);
            if (diary != null)
            {
                _context.Diaries.Remove(diary);
                await _context.SaveChangesAsync();
                return diary;
            }
            _logger.LogWarning("Diary deletion failed: diary with ID {DiaryId} not found.", id);
            return null;
        }
    }
}
