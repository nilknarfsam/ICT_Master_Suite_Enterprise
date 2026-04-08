using FluentValidation;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;

namespace ICTMasterSuite.Application.TechnicalHistory;

public sealed class SaveTechnicalAnalysisRequestValidator : AbstractValidator<SaveTechnicalAnalysisRequest>
{
    public SaveTechnicalAnalysisRequestValidator()
    {
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(120);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
        RuleFor(x => x.FilePath).NotEmpty().MaximumLength(1024);
        RuleFor(x => x.LogType).NotEmpty().MaximumLength(40);
        RuleFor(x => x.TechnicianName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.AnalysisText).NotEmpty().MaximumLength(4000);
    }
}
