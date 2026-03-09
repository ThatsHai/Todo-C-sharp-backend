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
                IAuthenticationService authService, 
                HttpContext httpContext) =>
            {
                var token = await authService.Login(loginRequest, httpContext);
                if (token == null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(new { token });
            });
        }
    }
}
