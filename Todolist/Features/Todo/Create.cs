using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Todo
{
    public class Create 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("", (NewTodoTask task, INewTodoTaskService service) =>
            {
                return service.CreateNewTask(task);
            });
        }
    }
}
