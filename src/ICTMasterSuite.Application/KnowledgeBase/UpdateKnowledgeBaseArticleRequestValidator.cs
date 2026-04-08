using FluentValidation;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;

namespace ICTMasterSuite.Application.KnowledgeBase;

public sealed class UpdateKnowledgeBaseArticleRequestValidator : AbstractValidator<UpdateKnowledgeBaseArticleRequest>
{
    public UpdateKnowledgeBaseArticleRequestValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.Model).NotEmpty().MaximumLength(120);
        RuleFor(x => x.TestPhase).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Symptom).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Solution).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(120);
    }
}
