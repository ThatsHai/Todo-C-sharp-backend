using Todolist.Services.Interfaces;


namespace Todolist.Features.Authentication
{
    public class RequireAuthorEP
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/testAuthor", async (
                IAuthenticationService authService) =>
            {
                return "you are admin";
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));
        }
    }
}
