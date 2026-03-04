using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Authentication
{
    public class Login
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (
                LoginRequest loginRequest,
                IAuthenticationService authService) =>
            {
                var token = await authService.Login(loginRequest);
                if (token == null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(new { token });
            });
        }
    }
}
