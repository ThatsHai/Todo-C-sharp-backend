using Todolist.Features.Authentication;

namespace Todolist.Models
{
    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
