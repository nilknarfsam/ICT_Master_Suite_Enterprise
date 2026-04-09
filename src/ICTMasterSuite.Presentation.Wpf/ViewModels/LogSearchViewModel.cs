using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Presentation.Wpf.Services;
using System.IO;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class LogSearchViewModel(
    ILogFinderService logFinderService,
    ISettingsService settingsService,
    ITechnicalHistoryService technicalHistoryService,
    FinderResultsState finderResultsState) : ObservableObject
{
    public ObservableCollection<LogFile> Logs { get; } = [];
    public ObservableCollection<string> Directories { get; } = [];
    public ObservableCollection<ParsedLog> Results { get; } = [];

    [ObservableProperty] private string searchTerm = string.Empty;
    [ObservableProperty] private string directoryInput = string.Empty;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private ParsedLog? selectedResult;
    [ObservableProperty] private bool hasPreviousHistory;
    [ObservableProperty] private bool showFinderEmptyState = true;

    public bool ShowFinderEmptyBanner => ShowFinderEmptyState && !IsLoading;

    partial void OnIsLoadingChanged(bool value) => OnPropertyChanged(nameof(ShowFinderEmptyBanner));

    partial void OnShowFinderEmptyStateChanged(bool value) => OnPropertyChanged(nameof(ShowFinderEmptyBanner));

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
        IsLoading = true;
        ShowFinderEmptyState = false;
        StatusMessage = "Pesquisando logs...";
        Results.Clear();
        Logs.Clear();

        try
        {
            var settings = await settingsService.LoadAsync();
            var configuredPaths = new[]
            {
                settings.CaminhoLogsTri,
                settings.CaminhoLogsAgilent,
                settings.BackupLocalDir
            };

            var searchDirectories = configuredPaths
                .Concat(Directories)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (searchDirectories.Count == 0)
            {
                StatusMessage = "Configure ao menos um diretorio (TRI, AGILENT ou BACKUP).";
                ShowFinderEmptyState = true;
                return;
            }

            var logs = await logFinderService.BuscarAsync(SearchTerm, searchDirectories);
            foreach (var log in logs)
            {
                Logs.Add(log);
                Results.Add(new ParsedLog(
                    sourceType: Path.GetExtension(log.Caminho).TrimStart('.').ToUpperInvariant(),
                    fileName: log.Nome,
                    fullPath: log.Caminho,
                    serialNumber: string.Empty,
                    model: string.Empty,
                    station: string.Empty,
                    logTimestamp: log.Data,
                    errorCode: string.Empty,
                    errorDescription: string.Empty,
                    result: LogResult.Fail,
                    analysedAt: DateTime.UtcNow,
                    summary: "Arquivo encontrado pelo Finder."));
            }

            finderResultsState.Set(Results.ToList());
            StatusMessage = $"{Logs.Count} arquivo(s) localizado(s).";
            ShowFinderEmptyState = Logs.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
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
