using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class CommentReactionsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CommentReactionsRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentReactionsRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public CommentReactionsRepository(AppDbContext context, ILogger<CommentReactionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all reactions for a specific comment.
        /// </summary>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>List of reactions for the comment.</returns>
        public async Task<IList<CommentReaction>> GetReactionsByCommentAsync(int commentId)
        {
            return await _context.CommentReactions
                .Where(r => r.CommentId == commentId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all reactions for comments in a specific diary.
        /// </summary>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>List of reactions for comments in the diary.</returns>
        public async Task<IList<CommentReaction>> GetReactionsByDiaryAsync(int diaryId)
        {
            return await _context.CommentReactions
                .Where(r => r.DiaryId == diaryId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an existing reaction for a profile and comment combination.
        /// </summary>
        /// <param name="commentId">Comment ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>Existing reaction if found, otherwise null.</returns>
        public async Task<CommentReaction?> GetExistingReactionAsync(int commentId, int profileId)
        {
            return await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.ProfileId == profileId);
        }

        /// <summary>
        /// Adds or updates a reaction for a comment.
        /// A profile can only have one reaction per comment (replaces existing).
        /// </summary>
        /// <param name="dto">Comment reaction data transfer object.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <param name="diaryId">Diary ID that contains the comment.</param>
        /// <param name="profileId">Profile ID of the user reacting.</param>
        /// <returns>The created or updated reaction.</returns>
        public async Task<CommentReaction> AddOrUpdateReactionAsync(CommentReactionDto dto, int commentId, int diaryId, int profileId)
        {
            // Validate reaction type (additional safety check)
            var validReactionTypes = new[] { "like", "love", "laugh", "sad", "angry", "surprised" };
            if (!validReactionTypes.Contains(dto.ReactionType.ToLower()))
            {
                throw new ArgumentException($"Invalid reaction type '{dto.ReactionType}'. Must be one of: {string.Join(", ", validReactionTypes)}");
            }

            // Check if reaction already exists
            var existingReaction = await GetExistingReactionAsync(commentId, profileId);

            if (existingReaction != null)
            {
                // Update existing reaction
                existingReaction.ReactionType = dto.ReactionType.ToLower(); // Normalize to lowercase
                await _context.SaveChangesAsync();
                return existingReaction;
            }
            else
            {
                // Create new reaction
                var reaction = new CommentReaction()
                {
                    CommentId = commentId,
                    DiaryId = diaryId,
                    ProfileId = profileId,
                    ReactionType = dto.ReactionType.ToLower() // Normalize to lowercase
                };

                await _context.CommentReactions.AddAsync(reaction);
                await _context.SaveChangesAsync();
                return reaction;
            }
        }

        /// <summary>
        /// Removes a reaction from a comment for a specific profile.
        /// </summary>
        /// <param name="commentId">Comment ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>Removed reaction if found, otherwise null.</returns>
        public async Task<CommentReaction?> RemoveReactionAsync(int commentId, int profileId)
        {
            var reaction = await GetExistingReactionAsync(commentId, profileId);
            
            if (reaction != null)
            {
                _context.CommentReactions.Remove(reaction);
                await _context.SaveChangesAsync();
                return reaction;
            }
            
            _logger.LogWarning("Reaction removal failed: no reaction found for comment {CommentId} and profile {ProfileId}.", commentId, profileId);
            return null;
        }
    }
}
