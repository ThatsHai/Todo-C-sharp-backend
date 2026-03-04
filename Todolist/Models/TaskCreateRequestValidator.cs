using FluentValidation;

namespace Todolist.Models
{
    public class TaskCreateRequestValidator : AbstractValidator<NewTodoTask>
    {
        public TaskCreateRequestValidator()
        {
            RuleFor(req => req.TaskUserId).NotEmpty();
            RuleFor(req => req.TaskName).NotEmpty();
        }
    }
}
