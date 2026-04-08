using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Logs.Dtos;
using ICTMasterSuite.Application.Logs.UseCases;
using ICTMasterSuite.Domain.Entities;
using System.IO;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class LogSearchViewModel(SearchLogsWithAnalysisUseCase useCase) : ObservableObject
{
    public ObservableCollection<string> Directories { get; } = [];
    public ObservableCollection<ParsedLog> Results { get; } = [];

    [ObservableProperty] private string searchTerm = string.Empty;
    [ObservableProperty] private string directoryInput = string.Empty;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string statusMessage = string.Empty;

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
}
