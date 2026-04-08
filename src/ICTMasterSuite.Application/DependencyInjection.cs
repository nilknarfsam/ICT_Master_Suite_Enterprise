using FluentValidation;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Configuration.UseCases;
using ICTMasterSuite.Application.Dashboard.UseCases;
using ICTMasterSuite.Application.KnowledgeBase.UseCases;
using ICTMasterSuite.Application.Logs.UseCases;
using ICTMasterSuite.Application.Reporting.UseCases;
using ICTMasterSuite.Application.Services;
using ICTMasterSuite.Application.TechnicalHistory.UseCases;
using ICTMasterSuite.Application.Updater.UseCases;
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
        services.AddScoped<ITechnicalHistoryService, TechnicalHistoryService>();
        services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddScoped<SearchLogsWithAnalysisUseCase>();
        services.AddScoped<SaveTechnicalAnalysisUseCase>();
        services.AddScoped<GetTechnicalHistoryBySerialUseCase>();
        services.AddScoped<SearchKnowledgeBaseUseCase>();
        services.AddScoped<CreateKnowledgeBaseArticleUseCase>();
        services.AddScoped<ExportTechnicalHistoryReportUseCase>();
        services.AddScoped<ExportKnowledgeBaseReportUseCase>();
        services.AddScoped<ExportFinderResultsReportUseCase>();
        services.AddScoped<GetDashboardSummaryUseCase>();
        services.AddScoped<CheckForUpdatesUseCase>();
        services.AddScoped<GetSystemConfigurationUseCase>();
        services.AddScoped<SaveSystemConfigurationUseCase>();
        return services;
    }
}
