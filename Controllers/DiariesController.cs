using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;
using lulu_diary_backend.Models.Database;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}/diaries")]
    public class DiariesController : ControllerBase
    {
        private readonly DiariesRepository _repository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly UserContext _userContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly FriendsRepository _friendsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiariesController"/> class.
        /// </summary>
        /// <param name="repository">Diaries repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="userContext">User context service.</param>
        /// <param name="authorizationService">Authorization service.</param>
        /// <param name="friendsRepository">Friends repository for friendship checks.</param>
        public DiariesController(DiariesRepository repository, ProfilesRepository profilesRepository, UserContext userContext, IAuthorizationService authorizationService, FriendsRepository friendsRepository)
        {
            _repository = repository;
            _profilesRepository = profilesRepository;
            _userContext = userContext;
            _authorizationService = authorizationService;
            _friendsRepository = friendsRepository;
        }

        /// <summary>
        /// Creates a new diary for the specified profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diary">Diary data transfer object.</param>
        /// <returns>Created diary.</returns>
        [HttpPost]
        [Authorize]
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

            // Check if user is authorized to create diaries for this profile
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, profile, "IsOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
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
        /// Gets all diaries for the specified profile with visibility protection.
        /// Applies profile diaryVisibility override and individual diary visibility rules.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>List of diaries for the profile that the current user can access.</returns>
        [HttpGet]
        public async Task<IActionResult> ListAsync(string username)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            // Check if profile has private diaryVisibility
            if (profile.DiaryVisibility == "private")
            {
                // Only the profile owner can see their diaries when diaryVisibility is private
                if (_userContext.CurrentUserProfile?.Id != profile.Id)
                {
                    return Ok(new List<object>()); // Return empty list for unauthorized users
                }
            }

            // Get all diaries for the profile
            var allDiaries = await _repository.GetDiariesByProfileAsync(profile.Id);
            
            // Filter diaries based on current user's access rights
            var accessibleDiaries = new List<object>();
            
            foreach (var diary in allDiaries)
            {
                if (await CanUserAccessDiary(diary, profile))
                {
                    accessibleDiaries.Add(diary);
                }
            }

            return Ok(accessibleDiaries);
        }

        /// <summary>
        /// Gets a specific diary by ID for the specified profile with visibility protection.
        /// Applies profile diaryVisibility override and individual diary visibility rules.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>Diary if found and accessible, otherwise NotFound or Forbidden.</returns>
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

            // Check if user can access this diary
            if (!await CanUserAccessDiary(result, profile))
            {
                return NotFound(); // Return NotFound instead of Forbidden for privacy
            }

            return Ok(result);
        }

        /// <summary>
        /// Checks if the current user can access a specific diary based on visibility rules.
        /// </summary>
        /// <param name="diary">The diary to check access for.</param>
        /// <param name="profile">The profile that owns the diary.</param>
        /// <returns>True if the user can access the diary, false otherwise.</returns>
        private async Task<bool> CanUserAccessDiary(Diary diary, Profile profile)
        {
            var currentUser = _userContext.CurrentUserProfile;
            
            // If profile has private diaryVisibility, only the owner can access
            if (profile.DiaryVisibility == "private")
            {
                return currentUser?.Id == profile.Id;
            }

            // Profile allows diary visibility, check individual diary rules
            switch (diary.Visibility)
            {
                case "public":
                    return true; // Public diaries are accessible to everyone

                case "friends-only":
                    if (currentUser == null)
                    {
                        return false; // Unauthenticated users can't see friends-only diaries
                    }
                    
                    if (currentUser.Id == profile.Id)
                    {
                        return true; // Owner can always see their own diaries
                    }

                    // Check if current user is friends with the diary owner
                    return await _friendsRepository.AreFriendsAsync(currentUser.Id, profile.Id);

                case "private":
                    // Private diaries are only accessible by the owner
                    return currentUser?.Id == profile.Id;

                default:
                    return false; // Unknown visibility, deny access
            }
        }

        /// <summary>
        /// Updates an existing diary.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <param name="diary">Diary update data transfer object.</param>
        /// <returns>Updated diary if found, otherwise NotFound.</returns>
        [HttpPatch("{diaryId}")]
        [Authorize]
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

            // Check if user is authorized to update this profile's diaries
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, profile, "IsOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
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
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="diaryId">Diary ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{diaryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(string username, int diaryId)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            // Check if user is authorized to delete this profile's diaries
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, profile, "IsOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
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
