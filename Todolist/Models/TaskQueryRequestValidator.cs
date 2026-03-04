using FluentValidation;

namespace Todolist.Models
{
    public class TaskQueryRequestValidator : AbstractValidator<TaskQueryRequest>
    {
        public TaskQueryRequestValidator() {
            RuleFor(req => req.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1.");

            RuleFor(req => req.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(req => req.Status)
                .Must(status => status == null
                    || status == "all"
                    || status == "pending"
                    || status == "completed")
                .WithMessage("Status must be 'pending' or 'completed'.");

            RuleFor(req => req.TaskName)
                .MaximumLength(100)
                .When(req => !string.IsNullOrWhiteSpace(req.TaskName))
                .WithMessage("TaskName cannot exceed 100 characters.");
        }
    }
}
