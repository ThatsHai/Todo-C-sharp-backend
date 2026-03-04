using Todolist.Models;

namespace Todolist.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public string GenerateJwt(string username);
        public Task<string> createUser(PersonMongo person);
        public Task<string?> Login(LoginRequest loginRequest);
    }
}
