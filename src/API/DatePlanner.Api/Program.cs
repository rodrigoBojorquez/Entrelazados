using System.Text.Json.Serialization;
using DatePlanner.Api.Common.Endpoints;
using DatePlanner.Api.Common.Extensions;
using DatePlanner.Api.Common.Middleware;
using DatePlanner.Api.Endpoints.TurnGames;
using DatePlanner.Application;
using DatePlanner.Application.Common.Notifications;
using DatePlanner.Infrastructure;
using DatePlanner.Infrastructure.Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddSingleton<TurnGamePresenceTracker>();
builder.Services.AddSingleton<ITurnGameNotifier, SignalRTurnGameNotifier>();

builder.Services.PostConfigure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        Func<MessageReceivedContext, Task> previousHandler = options.Events.OnMessageReceived;

        options.Events.OnMessageReceived = async context =>
        {
            await previousHandler(context);

            if (string.IsNullOrEmpty(context.Token) &&
                context.Request.Path.StartsWithSegments(TurnGameHubEndpoint.Route))
            {
                context.Token = context.Request.Query["access_token"];
            }
        };
    });

builder.Services.AddApplication();

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");

builder.Services.AddInfrastructure(builder.Configuration, databaseConnectionString);

builder.Services.AddEndpoints();

var app = builder.Build();

app.UseExceptionHandler();
app.UseLogContextTraceLogging();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("v1/swagger.json", "Entrelazados API V1"); });

app.ApplyMigrations();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();

public partial class Program;
