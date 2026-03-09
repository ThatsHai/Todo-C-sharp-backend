using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Entities;
using StackExchange.Redis;
using System.Text;
using Todolist.Core;
using Todolist.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
//builder.Services.AddSingleton<INewTodoTaskService, NewTodoTaskService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Password hasher
builder.Services.AddScoped<IPasswordHasher<PersonMongo>, PasswordHasher<PersonMongo>>();

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

// Services
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Authentication
var key = "THIS_IS_MY_SUPER_SECRET_KEY_12345";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // To let no expired token pass
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin only", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("IT domain only", policy =>
    {
        policy.RequireClaim("Domain", "IT");
    });
});

builder.Services.AddAuthorization();
//

var app = builder.Build();



app.UseCors("AllowAngular");
// Use authentication
app.UseAuthentication();
app.UseAuthorization();

// Mongo DB Initialization 
var db = await DB.InitAsync("todo-mongodb-image");

//Add module Todo
var modules = typeof(Program).Assembly
    .GetTypes()
    .Where(t => typeof(BaseModule).IsAssignableFrom(t) && !t.IsAbstract);

foreach (var type in modules)
{
    var module = (BaseModule)Activator.CreateInstance(type)!;
    module.Map(app);
}
//


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
