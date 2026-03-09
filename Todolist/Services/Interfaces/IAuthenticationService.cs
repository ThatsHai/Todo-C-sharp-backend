using Todolist.Models;

namespace Todolist.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public string GenerateJwt(string username);
        public Task<AuthResponse?> CreateUser(PersonMongo person, HttpContext httpContext);
        public Task<AuthResponse?> Login(LoginRequest loginRequest, HttpContext httpContext);
        public Task<AuthResponse> RefreshJwt(string refreshToken);
    }
}
