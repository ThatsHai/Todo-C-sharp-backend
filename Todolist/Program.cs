using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Entities;
using StackExchange.Redis;
using Todolist.Models;
using Todolist.Services;
using Todolist.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
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

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

var app = builder.Build();

// Mongo DB Initialization 
var db = await DB.InitAsync("todo-mongodb-image");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/newTasks", async (INewTodoTaskService service, [AsParameters] TaskQueryRequest request) =>
{
    return await service.GetNewTasks(request);
});

app.MapGet("/newTasks/{id}", (string id, INewTodoTaskService service) =>
{
    return service.GetNewTaskById(id);
});

app.MapPost("/newTasks", (NewTodoTask task, INewTodoTaskService service) =>
{
    return service.CreateNewTask(task);
});

app.MapDelete("/newTasks/{id}", async (INewTodoTaskService service, string id) =>
{
    await service.DeleteNewTask(id);
    return Results.NoContent();
});

app.MapPatch("/newTasks/toggle/{id}", async (INewTodoTaskService service, string id) =>
{
    await service.ToggleNewTask(id);
    return Results.Ok();
});

app.UseCors("AllowAngular");
app.UseHttpsRedirection();

app.Run();
