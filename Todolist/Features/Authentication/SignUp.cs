using FluentValidation.Results;
using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Authentication
{
    public class SignUP
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", async (
                PersonMongo person,
                IAuthenticationService authService,
                HttpContext httpContext) =>
            {
                PersonMongoValidator validator = new PersonMongoValidator();
                ValidationResult result = validator.Validate(person);
                if (!result.IsValid)
                {
                    return Results.BadRequest(result.Errors); ;
                }
                var token = await authService.CreateUser(person, httpContext);
                if (token == null)
                {
                    return Results.BadRequest(new { message = "User already exists" });
                }
                return Results.Ok(new { token });
            });
        }
    }
}
