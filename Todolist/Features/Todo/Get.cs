using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Todo
{
    public class Get 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/text", () => "HI");
            app.MapGet("/ping", () => "pong");
            app.MapGet("/{id}", (string id, INewTodoTaskService service) =>
            {
                return service.GetNewTaskById(id);
            });
        }
    }
}
