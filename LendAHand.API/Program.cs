using FluentValidation.AspNetCore;
using LendAHand.API;
using LendAHand.API.BackgroundJobs;
using LendAHand.API.Filters;
using LendAHand.API.Middleware;
using LendAHand.Application;
using LendAHand.Infrastructure;
using LendAHand.Infrastructure.Logging;
using Serilog;

// Serilog
SerilogConfiguration.Configure();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddEndpointsApiExplorer();

// Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// API Services — JWT + Swagger + Versioning + CORS
builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddHostedService<DueDateNotificationService>();


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();