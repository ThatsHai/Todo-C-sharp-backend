using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todolist.Models;
using Todolist.Services;
using Todolist.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
//builder.Services.AddScoped<UserService>();
builder.Services.AddProblemDetails();
//builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();
//builder.Services.AddScoped<INewTodoTaskService, NewTodoTaskService>();
builder.Services.AddSingleton<INewTodoTaskService, NewTodoTaskService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



var app = builder.Build();

app.UseCors("AllowAngular");
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var pds = context.RequestServices.GetRequiredService<IProblemDetailsService>();

        await pds.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = exception?.Message,
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            }
        });
    });
});


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/newTasks", async (INewTodoTaskService service, string? status, int page = 1, int pageSize = 1) =>
{
    if (status != null) return await Task.FromResult(service.GetNewTasks(status));
    return await service.GetAllNewTasks();
});

app.MapGet("/newTasks/users/{userId}", (string userId, INewTodoTaskService service) =>
{
    return service.GetTasksByUserId(userId);
});

app.MapGet("/newTasks/{id}", (string id, INewTodoTaskService service) =>
{
    return service.GetNewTaskById(id);
});

app.MapPost("/newTasks", (NewTodoTask task, INewTodoTaskService service) =>
{
    return service.CreateNewTask(task);
});

app.MapDelete("/newTasks/{id}", (string id, INewTodoTaskService service) =>
{
    return service.DeleteNewTask(id);
});


app.MapGet("/newTasks/toggle/{id}", (INewTodoTaskService service, string id) =>
{
    service.ToggleNewTask(id);
});


app.UseHttpsRedirection();

app.Run();
