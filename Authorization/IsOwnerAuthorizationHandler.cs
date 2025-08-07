using Microsoft.AspNetCore.Authorization;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Services;
using lulu_diary_backend.Context;
using Microsoft.EntityFrameworkCore;

namespace lulu_diary_backend.Authorization
{
    public class IsOwnerRequirement : IAuthorizationRequirement { }

    public class IsOwnerAuthorizationHandler : AuthorizationHandler<IsOwnerRequirement, object>
    {
        private readonly UserContext _userContext;
        private readonly AppDbContext _dbContext;

        public IsOwnerAuthorizationHandler(UserContext userContext, AppDbContext dbContext)
        {
            _userContext = userContext;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement, object resource)
        {
            if (_userContext.CurrentUserProfile == null)
            {
                return; // Must be authenticated to be an owner.
            }

            int? ownerProfileId = null;

            switch (resource)
            {
                case Profile profile:
                    ownerProfileId = profile.Id;
                    break;

                case Diary diary:
                    var diaryOwnerProfile = await _dbContext.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == diary.ProfileId);
                    if (diaryOwnerProfile != null)
                    {
                        ownerProfileId = diaryOwnerProfile.Id;
                    }
                    break;

                case Comment comment:
                    var commentOwnerProfile = await _dbContext.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.Id == comment.ProfileId);
                    if (commentOwnerProfile != null)
                    {
                        ownerProfileId = commentOwnerProfile.Id;
                    }
                    break;
            }

            if (ownerProfileId == _userContext.CurrentUserProfile.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}