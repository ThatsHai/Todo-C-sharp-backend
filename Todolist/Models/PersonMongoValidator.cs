using FluentValidation;

namespace Todolist.Models
{
    public class PersonMongoValidator : AbstractValidator<PersonMongo>
    {
        public PersonMongoValidator() {
            RuleFor(req => req.FirstName).NotEmpty();
            RuleFor(req => req.LastName).NotEmpty();
            RuleFor(req => req.UserName).NotEmpty();
            RuleFor(req => req.Password).NotEmpty();
        }
    }
}
