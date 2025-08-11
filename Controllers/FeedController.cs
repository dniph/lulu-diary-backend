using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/feed")]
    public class FeedController : ControllerBase
    {
        private readonly DiariesRepository _diariesRepository;
        private readonly FriendsRepository _friendsRepository;
        private readonly UserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedController"/> class.
        /// </summary>
        /// <param name="diariesRepository">Diaries repository.</param>
        /// <param name="friendsRepository">Friends repository.</param>
        /// <param name="userContext">User context service.</param>
        public FeedController(
            DiariesRepository diariesRepository,
            FriendsRepository friendsRepository,
            UserContext userContext)
        {
            _diariesRepository = diariesRepository;
            _friendsRepository = friendsRepository;
            _userContext = userContext;
        }

        /// <summary>
        /// Gets the feed based on authentication status.
        /// For unauthenticated users: returns public diaries only.
        /// For authenticated users: returns public diaries and friends-only diaries from friends, plus private diaries from own profile.
        /// </summary>
        /// <param name="limit">Maximum number of diaries to return (default: 20, max: 100).</param>
        /// <param name="offset">Number of diaries to skip for pagination (default: 0).</param>
        /// <param name="includeCount">Include total count for pagination (default: false).</param>
        /// <returns>List of diaries for the feed, optionally with total count.</returns>
        [HttpGet]
        public async Task<IActionResult> GetFeedAsync(
            [FromQuery] int limit = 20, 
            [FromQuery] int offset = 0,
            [FromQuery] bool includeCount = false)
        {
            // Validate parameters
            if (limit < 1 || limit > 100)
            {
                return BadRequest(new { message = "Limit must be between 1 and 100." });
            }

            if (offset < 0)
            {
                return BadRequest(new { message = "Offset cannot be negative." });
            }

            try
            {
                IList<Diary> diaries;
                int? totalCount = null;
                bool isAuthenticated = _userContext.CurrentUserProfile != null;

                if (isAuthenticated)
                {
                    // Authenticated user: public + friends-only from friends + private from own profile
                    var currentProfileId = _userContext.CurrentUserProfile!.Id;
                    diaries = await _diariesRepository.GetFeedDiariesAsync(currentProfileId, limit, offset);
                    
                    if (includeCount)
                    {
                        totalCount = await _diariesRepository.GetFeedDiariesCountAsync(currentProfileId);
                    }
                }
                else
                {
                    // Unauthenticated user: only public diaries
                    diaries = await _diariesRepository.GetPublicDiariesAsync(limit, offset);
                    
                    if (includeCount)
                    {
                        totalCount = await _diariesRepository.GetPublicDiariesCountAsync();
                    }
                }

                var response = new
                {
                    data = diaries,
                    pagination = new
                    {
                        limit,
                        offset,
                        count = diaries.Count,
                        totalCount = includeCount ? totalCount : null
                    },
                    meta = new
                    {
                        isAuthenticated
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the feed.", details = ex.Message });
            }
        }
    }
}
