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
                IAuthenticationService authService) =>
            {
                PersonMongoValidator validator = new PersonMongoValidator();
                ValidationResult result = validator.Validate(person);
                if (!result.IsValid)
                {
                    return Results.BadRequest(result.Errors); ;
                }
                var token = await authService.createUser(person);
                return Results.Ok(new { token });
            });
        }
    }
}
