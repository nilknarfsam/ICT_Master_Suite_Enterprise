using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Infrastructure.Persistence;
using ICTMasterSuite.Infrastructure.Persistence.Repositories;
using ICTMasterSuite.Infrastructure.Security;
using ICTMasterSuite.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ICTMasterSuite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.\\SQLEXPRESS;Database=ICTMasterSuite;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<IctMasterSuiteDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<IctMasterSuiteDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ISessionStore, InMemorySessionStore>();
        services.AddScoped<IAuditLogger, AuditLogger>();

        return services;
    }

    public static LoggerConfiguration AddCentralizedLogging(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .MinimumLevel.Information()
            .WriteTo.File("logs/ict-master-suite-.log", rollingInterval: RollingInterval.Day);
    }
}
