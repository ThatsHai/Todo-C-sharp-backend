using Todolist.Models;
using Todolist.Services.Interfaces;

namespace Todolist.Features.Todo
{
    public class GetList 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("", async (INewTodoTaskService service, [AsParameters] TaskQueryRequest request) =>
            {
                return await service.GetNewTasks(request);
            });
        }


    }
}
