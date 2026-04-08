using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.KnowledgeBase.Dtos;
using ICTMasterSuite.Application.Reporting.Dtos;
using ICTMasterSuite.Application.Reporting.UseCases;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class ReportsViewModel(
    ExportTechnicalHistoryReportUseCase exportTechnicalHistoryUseCase,
    ExportKnowledgeBaseReportUseCase exportKnowledgeBaseUseCase,
    ExportFinderResultsReportUseCase exportFinderResultsUseCase,
    FinderResultsState finderResultsState) : ObservableObject
{
    [ObservableProperty] private string serialInput = string.Empty;
    [ObservableProperty] private string modelFilter = string.Empty;
    [ObservableProperty] private string phaseFilter = string.Empty;
    [ObservableProperty] private string termFilter = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    [RelayCommand]
    private async Task ExportTechnicalHistoryExcelAsync() => await ExportTechnicalHistoryAsync(ReportFormat.Excel);

    [RelayCommand]
    private async Task ExportTechnicalHistoryPdfAsync() => await ExportTechnicalHistoryAsync(ReportFormat.Pdf);

    [RelayCommand]
    private async Task ExportKnowledgeBaseExcelAsync() => await ExportKnowledgeBaseAsync(ReportFormat.Excel);

    [RelayCommand]
    private async Task ExportKnowledgeBasePdfAsync() => await ExportKnowledgeBaseAsync(ReportFormat.Pdf);

    [RelayCommand]
    private async Task ExportFinderExcelAsync() => await ExportFinderAsync(ReportFormat.Excel);

    [RelayCommand]
    private async Task ExportFinderPdfAsync() => await ExportFinderAsync(ReportFormat.Pdf);

    public void ApplyKnowledgeFilter(string model, string term)
    {
        ModelFilter = model;
        TermFilter = term;
    }

    private async Task ExportTechnicalHistoryAsync(ReportFormat format)
    {
        var result = await exportTechnicalHistoryUseCase.ExecuteAsync(new TechnicalHistoryReportRequest(SerialInput, format));
        StatusMessage = result.IsSuccess ? $"Relatório gerado: {result.Value?.DisplayName}" : result.Message;
    }

    private async Task ExportKnowledgeBaseAsync(ReportFormat format)
    {
        var result = await exportKnowledgeBaseUseCase.ExecuteAsync(new KnowledgeBaseReportRequest(ModelFilter, PhaseFilter, TermFilter, format));
        StatusMessage = result.IsSuccess ? $"Relatório gerado: {result.Value?.DisplayName}" : result.Message;
    }

    private async Task ExportFinderAsync(ReportFormat format)
    {
        if (finderResultsState.LastResults.Count == 0)
        {
            StatusMessage = "Nenhum resultado do finder disponível para exportar.";
            return;
        }

        var result = await exportFinderResultsUseCase.ExecuteAsync(new FinderResultsReportRequest(finderResultsState.LastResults, format));
        StatusMessage = result.IsSuccess ? $"Relatório gerado: {result.Value?.DisplayName}" : result.Message;
    }
}
