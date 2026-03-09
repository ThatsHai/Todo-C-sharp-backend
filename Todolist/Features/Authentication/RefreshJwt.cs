using Microsoft.AspNetCore.Identity.Data;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Authentication
{
    public class RefreshJwt
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/refresh", async (
                RefreshRequest refreshToken,
                IAuthenticationService authService) =>
            {
                return await authService.RefreshJwt(refreshToken.RefreshToken);
            });
        }
    }
}
