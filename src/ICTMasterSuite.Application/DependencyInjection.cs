using FluentValidation;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ICTMasterSuite.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        return services;
    }
}
