using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class DiaryReactionsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DiaryReactionsRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiaryReactionsRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public DiaryReactionsRepository(AppDbContext context, ILogger<DiaryReactionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all reactions for a specific diary.
        /// </summary>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>List of reactions for the diary.</returns>
        public async Task<IList<DiaryReaction>> GetReactionsByDiaryAsync(int diaryId)
        {
            return await _context.DiaryReactions
                .Where(r => r.DiaryId == diaryId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an existing reaction for a profile and diary combination.
        /// </summary>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>Existing reaction if found, otherwise null.</returns>
        public async Task<DiaryReaction?> GetExistingReactionAsync(int diaryId, int profileId)
        {
            return await _context.DiaryReactions
                .FirstOrDefaultAsync(r => r.DiaryId == diaryId && r.ProfileId == profileId);
        }

        /// <summary>
        /// Adds or updates a reaction for a diary.
        /// A profile can only have one reaction per diary (replaces existing).
        /// </summary>
        /// <param name="dto">Diary reaction data transfer object.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="profileId">Profile ID of the user reacting.</param>
        /// <returns>The created or updated reaction.</returns>
        public async Task<DiaryReaction> AddOrUpdateReactionAsync(DiaryReactionDto dto, int diaryId, int profileId)
        {
            // Validate reaction type (additional safety check)
            var validReactionTypes = new[] { "like", "love", "hate" };
            if (!validReactionTypes.Contains(dto.ReactionType.ToLower()))
            {
                throw new ArgumentException($"Invalid reaction type '{dto.ReactionType}'. Must be one of: {string.Join(", ", validReactionTypes)}");
            }

            // Check if reaction already exists
            var existingReaction = await GetExistingReactionAsync(diaryId, profileId);

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
                var reaction = new DiaryReaction()
                {
                    DiaryId = diaryId,
                    ProfileId = profileId,
                    ReactionType = dto.ReactionType.ToLower() // Normalize to lowercase
                };

                await _context.DiaryReactions.AddAsync(reaction);
                await _context.SaveChangesAsync();
                return reaction;
            }
        }

        /// <summary>
        /// Removes a reaction from a diary for a specific profile.
        /// </summary>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="profileId">Profile ID.</param>
        /// <returns>Removed reaction if found, otherwise null.</returns>
        public async Task<DiaryReaction?> RemoveReactionAsync(int diaryId, int profileId)
        {
            var reaction = await GetExistingReactionAsync(diaryId, profileId);
            
            if (reaction != null)
            {
                _context.DiaryReactions.Remove(reaction);
                await _context.SaveChangesAsync();
                return reaction;
            }
            
            _logger.LogWarning("Reaction removal failed: no reaction found for diary {DiaryId} and profile {ProfileId}.", diaryId, profileId);
            return null;
        }
    }
}
