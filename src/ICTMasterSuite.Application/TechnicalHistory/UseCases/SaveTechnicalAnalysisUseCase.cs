using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;

namespace ICTMasterSuite.Application.TechnicalHistory.UseCases;

public sealed class SaveTechnicalAnalysisUseCase(ITechnicalHistoryService technicalHistoryService)
{
    public Task<Result<TechnicalAnalysisDto>> ExecuteAsync(SaveTechnicalAnalysisRequest request, CancellationToken cancellationToken = default)
        => technicalHistoryService.SaveTechnicalAnalysisAsync(request, cancellationToken);
}
