using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/profiles/{username}")]
    public class FollowersController : ControllerBase
    {
        private readonly FollowersRepository _followersRepository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly UserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowersController"/> class.
        /// </summary>
        /// <param name="followersRepository">Followers repository.</param>
        /// <param name="profilesRepository">Profiles repository for username lookups.</param>
        /// <param name="userContext">User context service.</param>
        public FollowersController(
            FollowersRepository followersRepository,
            ProfilesRepository profilesRepository,
            UserContext userContext)
        {
            _followersRepository = followersRepository;
            _profilesRepository = profilesRepository;
            _userContext = userContext;
        }

        /// <summary>
        /// Gets all followers for a specific profile.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>List of followers for the profile.</returns>
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowersAsync(string username)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var followers = await _followersRepository.GetFollowersAsync(profile.Id);
            return Ok(followers);
        }

        /// <summary>
        /// Gets all profiles that a specific profile is following.
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>List of profiles being followed.</returns>
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowingAsync(string username)
        {
            // Get profile by username
            var profile = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            var following = await _followersRepository.GetFollowingAsync(profile.Id);
            return Ok(following);
        }

        /// <summary>
        /// Follows a profile.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Username of the profile to follow.</param>
        /// <returns>Created follow relationship.</returns>
        [HttpPost("follow")]
        [Authorize]
        public async Task<IActionResult> FollowAsync(string username)
        {
            // Get profile by username (the profile to be followed)
            var profileToFollow = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profileToFollow == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            try
            {
                // Get current user's profile ID from UserContext
                var followerId = _userContext.CurrentUserProfile!.Id;

                var result = await _followersRepository.FollowProfileAsync(profileToFollow.Id, followerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the follow request.", details = ex.Message });
            }
        }

        /// <summary>
        /// Unfollows a profile.
        /// Middleware should ensure user is authenticated.
        /// </summary>
        /// <param name="username">Username of the profile to unfollow.</param>
        /// <returns>NoContent if unfollowed, otherwise NotFound.</returns>
        [HttpPost("unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowAsync(string username)
        {
            // Get profile by username (the profile to be unfollowed)
            var profileToUnfollow = await _profilesRepository.GetProfileByUsernameAsync(username);
            if (profileToUnfollow == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            try
            {
                // Get current user's profile ID from UserContext
                var followerId = _userContext.CurrentUserProfile!.Id;

                var result = await _followersRepository.UnfollowProfileAsync(profileToUnfollow.Id, followerId);
                if (result == null)
                {
                    return NotFound(new { message = "No follow relationship found to remove." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the unfollow request.", details = ex.Message });
            }
        }
    }
}
