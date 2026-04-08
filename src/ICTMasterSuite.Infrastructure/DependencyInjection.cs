using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Abstractions.Updater;
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
        services.AddScoped<ITechnicalAnalysisRepository, TechnicalAnalysisRepository>();
        services.AddScoped<IKnowledgeBaseArticleRepository, KnowledgeBaseArticleRepository>();
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ISessionStore, InMemorySessionStore>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<ILogFinderService, LogFinderService>();
        services.AddScoped<ILogParserService, LogParserService>();
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddSingleton<ICurrentApplicationVersionProvider, ReflectionCurrentApplicationVersionProvider>();
        services.AddHttpClient<IUpdateFeedClient, UpdateFeedClient>((_, client) =>
        {
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ICT-Master-Suite-Updater/1.0");
        });
        services.AddScoped<IUpdaterService, UpdaterService>();
        services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();

        return services;
    }

    public static LoggerConfiguration AddCentralizedLogging(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .MinimumLevel.Information()
            .WriteTo.File("logs/ict-master-suite-.log", rollingInterval: RollingInterval.Day);
    }
}
