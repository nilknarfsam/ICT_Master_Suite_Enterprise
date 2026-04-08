using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;
using ICTMasterSuite.Application.TechnicalHistory.UseCases;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class RegisterAnalysisViewModel(
    SaveTechnicalAnalysisUseCase saveTechnicalAnalysisUseCase,
    AuthenticatedUserState authenticatedUserState) : ObservableObject
{
    [ObservableProperty] private string serialNumber = string.Empty;
    [ObservableProperty] private string model = string.Empty;
    [ObservableProperty] private string fileName = string.Empty;
    [ObservableProperty] private string filePath = string.Empty;
    [ObservableProperty] private string logType = string.Empty;
    [ObservableProperty] private string errorCode = string.Empty;
    [ObservableProperty] private string errorDescription = string.Empty;
    [ObservableProperty] private string summary = string.Empty;
    [ObservableProperty] private string technicianName = string.Empty;
    [ObservableProperty] private string analysisText = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    private ParsedLog? _selectedLog;

    public event EventHandler<bool>? RequestClose;

    public void InitializeFromLog(ParsedLog log)
    {
        _selectedLog = log;
        SerialNumber = log.SerialNumber;
        Model = log.Model;
        FileName = log.FileName;
        FilePath = log.FullPath;
        LogType = log.SourceType;
        ErrorCode = log.ErrorCode;
        ErrorDescription = log.ErrorDescription;
        Summary = log.Summary;
        TechnicianName = authenticatedUserState.Current?.FullName ?? "Analista";
        AnalysisText = string.Empty;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_selectedLog is null)
        {
            StatusMessage = "Nenhum log selecionado.";
            return;
        }

        IsBusy = true;
        var result = await saveTechnicalAnalysisUseCase.ExecuteAsync(new SaveTechnicalAnalysisRequest(
            SerialNumber,
            Model,
            FileName,
            FilePath,
            LogType,
            _selectedLog.Result,
            ErrorCode,
            ErrorDescription,
            Summary,
            TechnicianName,
            string.IsNullOrWhiteSpace(AnalysisText) ? "Analise registrada sem observacoes adicionais." : AnalysisText));

        IsBusy = false;
        StatusMessage = result.IsSuccess ? "Análise registrada com sucesso." : result.Message;

        if (result.IsSuccess)
        {
            RequestClose?.Invoke(this, true);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(this, false);
    }
}
