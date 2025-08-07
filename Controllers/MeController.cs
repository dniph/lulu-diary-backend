using lulu_diary_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeController : ControllerBase
    { 
        private readonly UserContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeController"/> class.
        /// </summary>
        /// <param name="userContext"></param>
        public MeController(UserContext userContext)
        {
            _userContext = userContext;
        }

        /// <summary>
        /// Gets a profile by authenticated user
        /// </summary>
        /// <param name="username">Profile username.</param>
        /// <returns>Profile if found, otherwise NotFound.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            if (_userContext.CurrentUserProfile == null)
            {
                return BadRequest();
            }

            return Ok(_userContext.CurrentUserProfile);
        }
    }
}