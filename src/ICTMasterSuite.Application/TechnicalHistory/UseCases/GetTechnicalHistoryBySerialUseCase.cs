using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;

namespace ICTMasterSuite.Application.TechnicalHistory.UseCases;

public sealed class GetTechnicalHistoryBySerialUseCase(ITechnicalHistoryService technicalHistoryService)
{
    public Task<Result<IReadOnlyCollection<TechnicalAnalysisDto>>> ExecuteAsync(string serialNumber, CancellationToken cancellationToken = default)
        => technicalHistoryService.ListBySerialAsync(serialNumber, cancellationToken);
}
