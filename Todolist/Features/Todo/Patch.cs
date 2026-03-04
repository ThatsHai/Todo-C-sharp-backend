using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Todo
{
    public class Patch 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/toggle/{id}", async (INewTodoTaskService service, string id) =>
            {
                await service.ToggleNewTask(id);
                return Results.Ok();
            });
        }
    }
}
