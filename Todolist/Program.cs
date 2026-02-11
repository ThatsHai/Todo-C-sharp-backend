using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Entities;
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

var app = builder.Build();

// Mongo DB Initialization 
var db = await DB.InitAsync("todo-mongodb-image");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Welcome to the TodoList API!");

app.MapGet("/newTasks", async (INewTodoTaskService service, string? status, string? taskName, int pageSize = 10, int page = 1) =>
{
    return await service.GetNewTasks(status, taskName, page, pageSize);
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


app.MapGet("/newTasks/toggle/{id}", (INewTodoTaskService service, string id) =>
{
    service.ToggleNewTask(id);
});


app.MapGet("/test-person", async (string? firstName, string? lastName) =>
{
    //var person = new PersonMongo
    //{
    //    FirstName = "Jerry",
    //    LastName = "Tom"
    //};

    //await db.SaveAsync(person);

    //var retrievedPerson = await db.Find<PersonMongo>()
    //    .Match(p => p.ID == person.ID)
    //    .ExecuteFirstAsync();

    var retrievedPerson = await db.Find<PersonMongo>()
    .Match(p => (p.LastName == lastName) && (p.FirstName == firstName))
    .Project(p => p.Include(x => x.FirstName))
    .ExecuteAsync();
    return retrievedPerson;
    //return person;
});


app.UseCors("AllowAngular");
app.UseHttpsRedirection();

app.Run();
