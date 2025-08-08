using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries/{diaryId}/comments/{commentId}")]
    public class CommentReactionsController : ControllerBase
    {
        private readonly CommentReactionsRepository _repository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly DiariesRepository _diariesRepository;
        private readonly CommentsRepository _commentsRepository;
        private readonly UserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentReactionsController"/> class.
        /// </summary>
        /// <param name="repository">Comment reactions repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="diariesRepository">Diaries repository for diary validation.</param>
        /// <param name="commentsRepository">Comments repository for comment validation.</param>
        /// <param name="userContext">User context service.</param>
        public CommentReactionsController(
            CommentReactionsRepository repository, 
            ProfilesRepository profilesRepository,
            DiariesRepository diariesRepository,
            CommentsRepository commentsRepository,
            UserContext userContext)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
            _diariesRepository = diariesRepository;
            _commentsRepository = commentsRepository;
            _userContext = userContext;
        }

        /// <summary>
        /// Adds or updates a reaction to the specified comment.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <param name="reaction">Reaction data transfer object.</param>
        /// <returns>Created or updated reaction.</returns>
        [HttpPost("react")]
        [Authorize]
        public async Task<IActionResult> ReactAsync(string username, int diaryId, int commentId, CommentReactionDto reaction)
        {
            if (reaction == null)
            {
                return BadRequest();
            }

            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            // Verify diary exists and belongs to profile
            var diary = await _diariesRepository.GetDiaryByProfileAsync(diaryId, profile.Id);
            if (diary == null)
            {
                return NotFound(new { message = "Diary not found." });
            }

            // Verify comment exists and belongs to the diary
            var comment = await _commentsRepository.GetCommentAsync(commentId);
            if (comment == null || comment.DiaryId != diaryId)
            {
                return NotFound(new { message = "Comment not found." });
            }

            try
            {
                // Get current user's profile ID from UserContext
                var reactingProfileId = _userContext.CurrentUserProfile!.Id;

                var result = await _repository.AddOrUpdateReactionAsync(reaction, commentId, diaryId, reactingProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a reaction from the specified comment.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>NoContent if removed, otherwise NotFound.</returns>
        [HttpPost("unreact")]
        [Authorize]
        public async Task<IActionResult> UnreactAsync(string username, int diaryId, int commentId)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            // Verify diary exists and belongs to profile
            var diary = await _diariesRepository.GetDiaryByProfileAsync(diaryId, profile.Id);
            if (diary == null)
            {
                return NotFound(new { message = "Diary not found." });
            }

            // Verify comment exists and belongs to the diary
            var comment = await _commentsRepository.GetCommentAsync(commentId);
            if (comment == null || comment.DiaryId != diaryId)
            {
                return NotFound(new { message = "Comment not found." });
            }

            // Get current user's profile ID from UserContext
            var reactingProfileId = _userContext.CurrentUserProfile!.Id;

            var removedReaction = await _repository.RemoveReactionAsync(commentId, reactingProfileId);
            if (removedReaction == null)
            {
                return NotFound(new { message = "No reaction found to remove." });
            }

            return NoContent();
        }

        /// <summary>
        /// Gets all reactions for the specified comment.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>List of reactions for the comment.</returns>
        [HttpGet("reactions")]
        public async Task<IActionResult> GetReactionsAsync(string username, int diaryId, int commentId)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            // Verify diary exists and belongs to profile
            var diary = await _diariesRepository.GetDiaryByProfileAsync(diaryId, profile.Id);
            if (diary == null)
            {
                return NotFound(new { message = "Diary not found." });
            }

            // Verify comment exists and belongs to the diary
            var comment = await _commentsRepository.GetCommentAsync(commentId);
            if (comment == null || comment.DiaryId != diaryId)
            {
                return NotFound(new { message = "Comment not found." });
            }

            var reactions = await _repository.GetReactionsByCommentAsync(commentId);
            return Ok(reactions);
        }
    }
}
