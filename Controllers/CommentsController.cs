using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries/{diaryId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentsRepository _repository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly DiariesRepository _diariesRepository;
        private readonly UserContext _userContext;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsController"/> class.
        /// </summary>
        /// <param name="repository">Comments repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="diariesRepository">Diaries repository for diary validation.</param>
        /// <param name="userContext">User context service.</param>
        /// <param name="authorizationService">Authorization service.</param>
        public CommentsController(
            CommentsRepository repository, 
            ProfilesRepository profilesRepository,
            DiariesRepository diariesRepository,
            UserContext userContext,
            IAuthorizationService authorizationService)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
            _diariesRepository = diariesRepository;
            _userContext = userContext;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Creates a new comment for the specified diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="comment">Comment data transfer object.</param>
        /// <returns>Created comment.</returns>
        [HttpPost]
        [Authorize]
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
                // Get current user's profile ID from UserContext
                var commenterProfileId = _userContext.CurrentUserProfile!.Id;

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
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="commentId">Comment ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{commentId}")]
        [Authorize]
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

            // Get the comment to verify it exists
            var existingComment = await _repository.GetCommentByIdAsync(commentId);
            if (existingComment == null)
            {
                return NotFound(new { message = "Comment not found." });
            }

            // Check if current user is the owner of the comment
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingComment, "IsOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
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
