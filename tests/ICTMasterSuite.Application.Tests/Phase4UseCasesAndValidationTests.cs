using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;
using ICTMasterSuite.Application.KnowledgeBase.UseCases;
using ICTMasterSuite.Application.TechnicalHistory;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;
using ICTMasterSuite.Application.TechnicalHistory.UseCases;
using ICTMasterSuite.Domain.Enums;

namespace ICTMasterSuite.Application.Tests;

public class Phase4UseCasesAndValidationTests
{
    [Fact]
    public void SaveTechnicalAnalysisRequestValidator_ShouldRejectEmptyFields()
    {
        var validator = new SaveTechnicalAnalysisRequestValidator();
        var request = new SaveTechnicalAnalysisRequest("", "", "", "", "", LogResult.Fail, "", "", "", "", "");

        var validation = validator.Validate(request);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public async Task SaveTechnicalAnalysisUseCase_ShouldReturnSuccessThroughService()
    {
        var useCase = new SaveTechnicalAnalysisUseCase(new FakeTechnicalHistoryService());
        var request = new SaveTechnicalAnalysisRequest("SN1", "M1", "f.log", "c:\\f.log", "TRI", LogResult.Fail, "E1", "desc", "sum", "tec", "analysis");

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task CreateKnowledgeBaseArticleUseCase_ShouldReturnSuccessThroughService()
    {
        var useCase = new CreateKnowledgeBaseArticleUseCase(new FakeKnowledgeBaseService());
        var result = await useCase.ExecuteAsync(new CreateKnowledgeBaseArticleRequest("M1", "FCT", "No power", "Replace fuse", "Tech"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    private sealed class FakeTechnicalHistoryService : ITechnicalHistoryService
    {
        public Task<Result<TechnicalAnalysisDto>> SaveTechnicalAnalysisAsync(SaveTechnicalAnalysisRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<TechnicalAnalysisDto>.Success(
                new TechnicalAnalysisDto(Guid.NewGuid(), request.SerialNumber, request.Model, request.FileName, request.FilePath, request.LogType, request.Result, request.ErrorCode, request.ErrorDescription, request.Summary, request.TechnicianName, request.AnalysisText, DateTime.UtcNow, null)));

        public Task<Result<IReadOnlyCollection<TechnicalAnalysisDto>>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyCollection<TechnicalAnalysisDto>>.Success(Array.Empty<TechnicalAnalysisDto>()));

        public Task<Result<TechnicalAnalysisDto>> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<TechnicalAnalysisDto>.Failure("not found"));
    }

    private sealed class FakeKnowledgeBaseService : IKnowledgeBaseService
    {
        public Task<Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>> SearchAsync(SearchKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>.Success(Array.Empty<KnowledgeBaseArticleDto>()));

        public Task<Result<KnowledgeBaseArticleDto>> CreateAsync(CreateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<KnowledgeBaseArticleDto>.Success(
                new KnowledgeBaseArticleDto(Guid.NewGuid(), request.Model, request.TestPhase, request.Symptom, request.Solution, request.Author, true, DateTime.UtcNow, null)));

        public Task<Result<KnowledgeBaseArticleDto>> UpdateAsync(UpdateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<KnowledgeBaseArticleDto>.Failure("not used"));

        public Task<Result> SetActiveStatusAsync(Guid articleId, bool isActive, CancellationToken cancellationToken = default)
            => Task.FromResult(Result.Success());
    }
}
