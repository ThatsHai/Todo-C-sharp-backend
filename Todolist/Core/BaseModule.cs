using System.Reflection;

namespace Todolist.Core
{
    public abstract class BaseModule(string prefix)
    {
        private readonly string _prefix = prefix;


        public void Map(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup($"/{_prefix}");
            Register(group);
        }

        protected abstract void Register(IEndpointRouteBuilder group);
        protected void AutoRegister(IEndpointRouteBuilder group)
        {
            var moduleNamespace = GetType().Namespace;
            var assembly = Assembly.GetExecutingAssembly();

            var endpointTypes = assembly.GetTypes()
                .Where(t =>
                    t is { IsAbstract: false, IsInterface: false } &&
                    //typeof(IModuleEndpoint).IsAssignableFrom(t) &&
                    t.Namespace == moduleNamespace
                );

            foreach (var type in endpointTypes)
            {
                //var endpoint = (IModuleEndpoint)Activator.CreateInstance(type)!;
                //endpoint.AddRoutes(group);

                var instance = Activator.CreateInstance(type);
                var method = type.GetMethod("AddRoutes");
                //validate
                if (method == null)
                {
                    continue;
                }
                var parameters = method.GetParameters();

                var isValid =
                    method.ReturnType == typeof(void) &&
                    parameters.Length == 1 &&
                    typeof(IEndpointRouteBuilder).IsAssignableFrom(parameters[0]
                    .ParameterType);

                if (!isValid)
                    continue;
                //
                method?.Invoke(instance, new Object[] { group });
            }
        }
    }
}
