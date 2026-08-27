using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Application.Common.Messaging;
using DatePlanner.Infrastructure.Common.Authentication;
using DatePlanner.Infrastructure.Common.Authorization;
using DatePlanner.Infrastructure.Common.Data;
using DatePlanner.Infrastructure.Database;
using DatePlanner.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace DatePlanner.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string databaseConnectionString)
    {
        services.AddAuthenticationInternal();

        services.AddAuthorizationInternal();
        
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());
        
        services.AddDbContext<DatePlannerDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.DefaultSchema))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatePlannerDbContext>());

        // services.AddScoped<IEventRepository, EventRepository>();
        // services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        // services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.Configure<OutboxOptions>(configuration.GetSection("Events:Outbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        
        return services;
    }
    
    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();
        
        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);
        
            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();
            
            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);
        
            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }
}