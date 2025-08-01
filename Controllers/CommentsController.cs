using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries/{diaryId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentsRepository _repository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly DiariesRepository _diariesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsController"/> class.
        /// </summary>
        /// <param name="repository">Comments repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="diariesRepository">Diaries repository for diary validation.</param>
        public CommentsController(
            CommentsRepository repository, 
            ProfilesRepository profilesRepository,
            DiariesRepository diariesRepository)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
            _diariesRepository = diariesRepository;
        }

        /// <summary>
        /// Creates a new comment for the specified diary.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="comment">Comment data transfer object.</param>
        /// <returns>Created comment.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(string username, int diaryId, CommentDto comment)
        {
            if (comment == null)
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

            try
            {
                // TODO: Get actual profileId from middleware-injected user context
                var commenterProfileId = 1; // placeholder - should come from authenticated user

                var result = await _repository.InsertCommentAsync(comment, diaryId, commenterProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets all comments for the specified diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>List of comments for the diary.</returns>
        [HttpGet]
        public async Task<IActionResult> ListAsync(string username, int diaryId)
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

            var results = await _repository.GetCommentsByDiaryAsync(diaryId);
            return Ok(results);
        }

        /// <summary>
        /// Deletes a comment by ID.
        /// Middleware should ensure user is authorized to delete this comment.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteAsync(string username, int diaryId, int commentId)
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

            var comment = await _repository.DeleteCommentAsync(commentId, diaryId);
            if (comment == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
