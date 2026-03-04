using Todolist.Core;

namespace Todolist.Features.Todo;

public class TodoModuleInhe() : BaseModule("newTasks")
{
    protected override void Register(IEndpointRouteBuilder group)
    {
        AutoRegister(group);
    }

}