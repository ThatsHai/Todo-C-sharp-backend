using Todolist.Services.Interfaces;


namespace Todolist.Features.Authentication
{
    public class RequireAuthenEP
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/testAuthen", async (
                IAuthenticationService authService) =>
            {
                return Results.Ok(new { message = "authenticated" });
            }).RequireAuthorization();
        }
    }
}
