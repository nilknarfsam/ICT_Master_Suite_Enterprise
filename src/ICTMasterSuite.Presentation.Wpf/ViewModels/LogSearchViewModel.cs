using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Logs.Dtos;
using ICTMasterSuite.Application.Logs.UseCases;
using ICTMasterSuite.Domain.Entities;
using System.IO;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class LogSearchViewModel(
    SearchLogsWithAnalysisUseCase useCase,
    ITechnicalHistoryService technicalHistoryService) : ObservableObject
{
    public ObservableCollection<string> Directories { get; } = [];
    public ObservableCollection<ParsedLog> Results { get; } = [];

    [ObservableProperty] private string searchTerm = string.Empty;
    [ObservableProperty] private string directoryInput = string.Empty;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private ParsedLog? selectedResult;
    [ObservableProperty] private bool hasPreviousHistory;

    public event EventHandler<ParsedLog>? RegisterAnalysisRequested;
    public event EventHandler<string>? ViewHistoryRequested;
    public event EventHandler<(string Model, string Term)>? SearchKnowledgeRequested;

    [RelayCommand]
    private void AddDirectory()
    {
        var trimmed = DirectoryInput.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || !Directory.Exists(trimmed))
        {
            StatusMessage = "Diretorio invalido.";
            return;
        }

        if (!Directories.Contains(trimmed))
        {
            Directories.Add(trimmed);
        }

        DirectoryInput = string.Empty;
    }

    [RelayCommand]
    private void RemoveDirectory(string? directory)
    {
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directories.Remove(directory);
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (Directories.Count == 0)
        {
            StatusMessage = "Adicione ao menos um diretorio.";
            return;
        }

        IsLoading = true;
        StatusMessage = "Pesquisando e analisando logs...";
        Results.Clear();

        var request = new SearchLogsWithAnalysisRequest(SearchTerm, Directories.ToList());
        var parsed = await useCase.ExecuteAsync(request);

        foreach (var item in parsed)
        {
            Results.Add(item);
        }

        StatusMessage = $"{Results.Count} logs analisados.";
        IsLoading = false;
    }

    [RelayCommand]
    private void RegisterAnalysis()
    {
        if (SelectedResult is null)
        {
            StatusMessage = "Selecione um log para registrar analise.";
            return;
        }

        RegisterAnalysisRequested?.Invoke(this, SelectedResult);
    }

    partial void OnSelectedResultChanged(ParsedLog? value)
    {
        if (value is null)
        {
            HasPreviousHistory = false;
            return;
        }

        _ = CheckHistoryAsync(value.SerialNumber);
    }

    private async Task CheckHistoryAsync(string serialNumber)
    {
        var latest = await technicalHistoryService.GetLatestBySerialAsync(serialNumber);
        HasPreviousHistory = latest.IsSuccess;
        if (HasPreviousHistory)
        {
            StatusMessage = $"Alerta: serial {serialNumber} ja possui historico tecnico.";
        }
    }

    [RelayCommand]
    private void OpenHistory()
    {
        if (SelectedResult is null)
        {
            StatusMessage = "Selecione um log para abrir o histórico.";
            return;
        }

        ViewHistoryRequested?.Invoke(this, SelectedResult.SerialNumber);
    }

    [RelayCommand]
    private void SearchRelatedKnowledge()
    {
        if (SelectedResult is null)
        {
            StatusMessage = "Selecione um log para buscar conhecimento relacionado.";
            return;
        }

        SearchKnowledgeRequested?.Invoke(this, (SelectedResult.Model, SelectedResult.ErrorDescription));
    }
}
