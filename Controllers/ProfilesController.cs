using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly ProfilesRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesController"/> class.
        /// </summary>
        /// <param name="repository">Profiles repository.</param>
        public ProfilesController(ProfilesRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new profile for the authenticated user.
        /// </summary>
        /// <param name="profile">Profile data transfer object.</param>
        /// <returns>Created profile.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProfileDto profile)
        {
            if (profile == null)
            {
                return BadRequest();
            }

            // TODO: Get userId from middleware-injected user context
            var userId = "placeholder-user-id";

            try
            {
                var result = await _repository.InsertProfileAsync(profile, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a profile by username.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>Profile if found, otherwise NotFound.</returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetAsync(string username)
        {
            var result = await _repository.GetProfileByUsernameAsync(username);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing profile.
        /// Middleware should ensure user is authorized to update this profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <param name="profile">Profile update data transfer object.</param>
        /// <returns>Updated profile if found, otherwise NotFound.</returns>
        [HttpPatch("{username}")]
        public async Task<IActionResult> UpdateAsync(string username, ProfileUpdateDto profile)
        {
            if (profile == null)
            {
                return BadRequest();
            }

            // TODO: Get userId from middleware-injected user context
            var userId = "placeholder-user-id";

            var existingProfile = await _repository.UpdateProfileAsync(username, profile, userId);
            if (existingProfile == null)
            {
                return NotFound();
            }

            return Ok(existingProfile);
        }
    }
}
