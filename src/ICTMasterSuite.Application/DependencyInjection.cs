using FluentValidation;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.KnowledgeBase.UseCases;
using ICTMasterSuite.Application.Logs.UseCases;
using ICTMasterSuite.Application.Services;
using ICTMasterSuite.Application.TechnicalHistory.UseCases;
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
        return services;
    }
}
