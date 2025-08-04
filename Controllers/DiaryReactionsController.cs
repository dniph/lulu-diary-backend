using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries/{diaryId}")]
    public class DiaryReactionsController : ControllerBase
    {
        private readonly DiaryReactionsRepository _repository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly DiariesRepository _diariesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiaryReactionsController"/> class.
        /// </summary>
        /// <param name="repository">Diary reactions repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="diariesRepository">Diaries repository for diary validation.</param>
        public DiaryReactionsController(
            DiaryReactionsRepository repository, 
            ProfilesRepository profilesRepository,
            DiariesRepository diariesRepository)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
            _diariesRepository = diariesRepository;
        }

        /// <summary>
        /// Adds or updates a reaction to the specified diary.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="reaction">Reaction data transfer object.</param>
        /// <returns>Created or updated reaction.</returns>
        [HttpPost("react")]
        public async Task<IActionResult> ReactAsync(string username, int diaryId, DiaryReactionDto reaction)
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

            try
            {
                // TODO: Get actual profileId from middleware-injected user context
                var reactingProfileId = 1; // placeholder - should come from authenticated user

                var result = await _repository.AddOrUpdateReactionAsync(reaction, diaryId, reactingProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a reaction from the specified diary.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>NoContent if removed, otherwise NotFound.</returns>
        [HttpPost("unreact")]
        public async Task<IActionResult> UnreactAsync(string username, int diaryId)
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

            // TODO: Get actual profileId from middleware-injected user context
            var reactingProfileId = 1; // placeholder - should come from authenticated user

            var removedReaction = await _repository.RemoveReactionAsync(diaryId, reactingProfileId);
            if (removedReaction == null)
            {
                return NotFound(new { message = "No reaction found to remove." });
            }

            return NoContent();
        }

        /// <summary>
        /// Gets all reactions for the specified diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>List of reactions for the diary.</returns>
        [HttpGet("reactions")]
        public async Task<IActionResult> GetReactionsAsync(string username, int diaryId)
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

            var reactions = await _repository.GetReactionsByDiaryAsync(diaryId);
            return Ok(reactions);
        }
    }
}
