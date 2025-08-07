using lulu_diary_backend.Models.Database;

namespace lulu_diary_backend.Services
{
    public class UserContext
    {
        public Profile? CurrentUserProfile { get; set; }
    }
}
