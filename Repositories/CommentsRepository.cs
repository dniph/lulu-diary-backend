using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class CommentsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CommentsRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public CommentsRepository(AppDbContext context, ILogger<CommentsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all comments for a specific diary.
        /// </summary>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>List of comments for the diary ordered by creation time.</returns>
        public async Task<IList<Comment>> GetCommentsByDiaryAsync(int diaryId)
        {
            return await _context.Comments
                .Where(c => c.DiaryId == diaryId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Inserts a new comment into the database.
        /// </summary>
        /// <param name="dto">Comment data transfer object.</param>
        /// <param name="diaryId">Diary ID the comment belongs to.</param>
        /// <param name="profileId">Profile ID of the comment author.</param>
        /// <returns>The created comment.</returns>
        public async Task<Comment> InsertCommentAsync(CommentDto dto, int diaryId, int profileId)
        {
            var comment = new Comment()
            {
                Content = dto.Content,
                DiaryId = diaryId,
                ProfileId = profileId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        /// <summary>
        /// Gets a comment by its ID.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <returns>Comment if found, otherwise null.</returns>
        public async Task<Comment?> GetCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", id);
            }
            return comment;
        }

        /// <summary>
        /// Gets a comment by its ID and diary ID.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>Comment if found and belongs to diary, otherwise null.</returns>
        public async Task<Comment?> GetCommentByDiaryAsync(int id, int diaryId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.DiaryId == diaryId);
            
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found for diary {DiaryId}.", id, diaryId);
            }
            return comment;
        }

        /// <summary>
        /// Updates an existing comment in the database.
        /// Authorization should be handled by middleware before reaching this method.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <param name="dto">Comment update data transfer object.</param>
        /// <param name="diaryId">Diary ID for verification.</param>
        /// <returns>Updated comment if found, otherwise null.</returns>
        public async Task<Comment?> UpdateCommentAsync(int id, CommentUpdateDto dto, int diaryId)
        {
            var existingComment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.DiaryId == diaryId);
            
            if (existingComment == null)
            {
                _logger.LogWarning("Comment update failed: comment with ID {CommentId} not found for diary {DiaryId}.", id, diaryId);
                return null;
            }

            // Update only the provided fields
            if (dto.Content != null)
            {
                existingComment.Content = dto.Content;
            }

            existingComment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existingComment;
        }

        /// <summary>
        /// Deletes a comment from the database by its ID.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <param name="diaryId">Diary ID for verification.</param>
        /// <returns>Deleted comment if found, otherwise null.</returns>
        public async Task<Comment?> DeleteCommentAsync(int id, int diaryId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.DiaryId == diaryId);
            
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return comment;
            }
            
            _logger.LogWarning("Comment deletion failed: comment with ID {CommentId} not found for diary {DiaryId}.", id, diaryId);
            return null;
        }
    }
}
