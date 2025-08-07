using lulu_diary_backend.Repositories;
using lulu_diary_backend.Services;
using System.Security.Claims;

namespace lulu_diary_backend.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, UserContext userContext, ProfilesRepository profilesRepository)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                userContext.CurrentUserProfile = await profilesRepository.GetProfileByUserIdAsync(userId);

                if (userContext.CurrentUserProfile == null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await httpContext.Response.WriteAsync("Internal Server Error: User profile not found.");
                    return;
                }
            }

            await _next(httpContext);
        }
    }
}
