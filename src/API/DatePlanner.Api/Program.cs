using DatePlanner.Api.Common.Extensions;
using DatePlanner.Api.Common.Middleware;
using DatePlanner.Application;
using DatePlanner.Infrastructure;
using DatePlanner.Infrastructure.Common.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddApplication();

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");

builder.Services.AddInfrastructure(builder.Configuration, databaseConnectionString);

var app = builder.Build();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();

app.Run();