using Todolist.Services.Interfaces;


namespace Todolist.Features.Authentication
{
    public class RequireITDomainClaimEP
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login/testClaim", async (
                IAuthenticationService authService) =>
            {
                return "you are IT";
            }).RequireAuthorization("IT domain only");
        }
    }
}
