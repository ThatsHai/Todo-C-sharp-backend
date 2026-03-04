using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Todo
{
    public class Delete 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/{id}", async (INewTodoTaskService service, string id) =>
            {
                await service.DeleteNewTask(id);
                return Results.NoContent();
            });
        }
    }
}
