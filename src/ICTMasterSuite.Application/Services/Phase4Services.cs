using FluentValidation;
using ICTMasterSuite.Application.Abstractions.Infrastructure;
using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;
using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Services;

public sealed class TechnicalHistoryService(
    ITechnicalAnalysisRepository repository,
    IAuditLogger auditLogger,
    IAppDbContext dbContext,
    IValidator<SaveTechnicalAnalysisRequest> validator) : ITechnicalHistoryService
{
    public async Task<Result<TechnicalAnalysisDto>> SaveTechnicalAnalysisAsync(SaveTechnicalAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<TechnicalAnalysisDto>.Failure(validation.ToString());
        }

        var entity = new TechnicalAnalysis(
            request.SerialNumber,
            request.Model,
            request.FileName,
            request.FilePath,
            request.LogType,
            request.Result,
            request.ErrorCode,
            request.ErrorDescription,
            request.Summary,
            request.TechnicianName,
            request.AnalysisText);

        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("technical-analysis.create", $"AnalysisId: {entity.Id}; Serial: {entity.SerialNumber}", cancellationToken);
        return Result<TechnicalAnalysisDto>.Success(Map(entity));
    }

    public async Task<Result<IReadOnlyCollection<TechnicalAnalysisDto>>> ListBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var rows = await repository.ListBySerialAsync(serialNumber, cancellationToken);
        return Result<IReadOnlyCollection<TechnicalAnalysisDto>>.Success(rows.Select(Map).ToList());
    }

    public async Task<Result<TechnicalAnalysisDto>> GetLatestBySerialAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var row = await repository.GetLatestBySerialAsync(serialNumber, cancellationToken);
        return row is null
            ? Result<TechnicalAnalysisDto>.Failure("Nenhuma analise encontrada.")
            : Result<TechnicalAnalysisDto>.Success(Map(row));
    }

    private static TechnicalAnalysisDto Map(TechnicalAnalysis x) => new(
        x.Id, x.SerialNumber, x.Model, x.FileName, x.FilePath, x.LogType, x.Result, x.ErrorCode,
        x.ErrorDescription, x.Summary, x.TechnicianName, x.AnalysisText, x.CreatedAt, x.UpdatedAt);
}

public sealed class KnowledgeBaseService(
    IKnowledgeBaseArticleRepository repository,
    IAuditLogger auditLogger,
    IAppDbContext dbContext,
    IValidator<CreateKnowledgeBaseArticleRequest> createValidator,
    IValidator<UpdateKnowledgeBaseArticleRequest> updateValidator) : IKnowledgeBaseService
{
    public async Task<Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>> SearchAsync(SearchKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        var rows = await repository.SearchAsync(request.Model, request.TestPhase, request.Term, request.IncludeInactive, cancellationToken);
        return Result<IReadOnlyCollection<KnowledgeBaseArticleDto>>.Success(rows.Select(Map).ToList());
    }

    public async Task<Result<KnowledgeBaseArticleDto>> CreateAsync(CreateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<KnowledgeBaseArticleDto>.Failure(validation.ToString());
        }

        var entity = new KnowledgeBaseArticle(request.Model, request.TestPhase, request.Symptom, request.Solution, request.Author);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("knowledge-base.create", $"ArticleId: {entity.Id}", cancellationToken);
        return Result<KnowledgeBaseArticleDto>.Success(Map(entity));
    }

    public async Task<Result<KnowledgeBaseArticleDto>> UpdateAsync(UpdateKnowledgeBaseArticleRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<KnowledgeBaseArticleDto>.Failure(validation.ToString());
        }

        var entity = await repository.GetByIdAsync(request.ArticleId, cancellationToken);
        if (entity is null)
        {
            return Result<KnowledgeBaseArticleDto>.Failure("Artigo nao encontrado.");
        }

        entity.Update(request.Model, request.TestPhase, request.Symptom, request.Solution, request.Author);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("knowledge-base.update", $"ArticleId: {entity.Id}", cancellationToken);
        return Result<KnowledgeBaseArticleDto>.Success(Map(entity));
    }

    public async Task<Result> SetActiveStatusAsync(Guid articleId, bool isActive, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(articleId, cancellationToken);
        if (entity is null)
        {
            return Result.Failure("Artigo nao encontrado.");
        }

        entity.SetActive(isActive);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogger.WriteAsync("knowledge-base.status", $"ArticleId: {entity.Id}; Active: {isActive}", cancellationToken);
        return Result.Success();
    }

    private static KnowledgeBaseArticleDto Map(KnowledgeBaseArticle x)
        => new(x.Id, x.Model, x.TestPhase, x.Symptom, x.Solution, x.Author, x.IsActive, x.CreatedAt, x.UpdatedAt);
}
