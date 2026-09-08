using DatePlanner.Application.Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DatePlanner.Infrastructure.Common.Authentication;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration(AuthenticationOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key) && options.Key.Length >= 32,
                "Authentication:Key debe contener al menos 32 caracteres.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Authentication:Issuer es obligatorio.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                "Authentication:Audience es obligatorio.")
            .Validate(options => options.JwtExpireMinutes > 0,
                "Authentication:JwtExpireMinutes debe ser mayor que cero.")
            .Validate(options => options.RefreshTokenExpireDays > 0,
                "Authentication:RefreshTokenExpireDays debe ser mayor que cero.")
            .ValidateOnStart();

        services.AddAuthorization();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddHttpContextAccessor();

        services.ConfigureOptions<JwtBearerConfigureOptions>();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }
}
