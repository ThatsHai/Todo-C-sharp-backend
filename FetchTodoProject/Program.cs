using FetchTodoProject.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<FetchTodoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/fetchLocalTodo", (FetchTodoService service) =>
{
    return service.FetchTodos();
});

app.Run();

