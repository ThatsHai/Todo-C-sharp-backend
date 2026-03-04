using Todolist.Core;

namespace Todolist.Features.Authentication
{
    public class Module() : BaseModule("authen")
    {
        protected override void Register(IEndpointRouteBuilder group)
        {
            AutoRegister(group);
        }
    }
}
