using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries")]
    public class DiariesController : ControllerBase
    {
        private readonly DiariesRepository _repository;
        private readonly ProfilesRepository _profilesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiariesController"/> class.
        /// </summary>
        /// <param name="repository">Diaries repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        public DiariesController(DiariesRepository repository, ProfilesRepository profilesRepository)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
        }

        /// <summary>
        /// Creates a new diary for the specified profile.
        /// Middleware should ensure user is authorized to create diaries for this profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diary">Diary data transfer object.</param>
        /// <returns>Created diary.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(string username, DiaryDto diary)
        {
            if (diary == null)
            {
                return BadRequest();
            }

            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            try
            {
                var result = await _repository.InsertDiaryAsync(diary, profile.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets all diaries for the specified profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>List of diaries for the profile.</returns>
        [HttpGet]
        public async Task<IActionResult> ListAsync(string username)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var results = await _repository.GetDiariesByProfileAsync(profile.Id);
            return Ok(results);
        }

        /// <summary>
        /// Gets a specific diary by ID for the specified profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>Diary if found, otherwise NotFound.</returns>
        [HttpGet("{diaryId}")]
        public async Task<IActionResult> GetAsync(string username, int diaryId)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var result = await _repository.GetDiaryByProfileAsync(diaryId, profile.Id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing diary.
        /// Middleware should ensure user is authorized to update this diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="diary">Diary update data transfer object.</param>
        /// <returns>Updated diary if found, otherwise NotFound.</returns>
        [HttpPatch("{diaryId}")]
        public async Task<IActionResult> UpdateAsync(string username, int diaryId, DiaryUpdateDto diary)
        {
            if (diary == null)
            {
                return BadRequest();
            }

            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var existingDiary = await _repository.UpdateDiaryAsync(diaryId, diary, profile.Id);
            if (existingDiary == null)
            {
                return NotFound();
            }

            return Ok(existingDiary);
        }

        /// <summary>
        /// Deletes a diary by ID.
        /// Middleware should ensure user is authorized to delete this diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{diaryId}")]
        public async Task<IActionResult> DeleteAsync(string username, int diaryId)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var diary = await _repository.DeleteDiaryAsync(diaryId, profile.Id);
            if (diary == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
