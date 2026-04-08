using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.TechnicalHistory.UseCases;
using ICTMasterSuite.Application.TechnicalHistory.Dtos;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class TechnicalHistoryViewModel(GetTechnicalHistoryBySerialUseCase useCase) : ObservableObject
{
    public ObservableCollection<TechnicalAnalysisDto> Analyses { get; } = [];

    [ObservableProperty] private string serialSearch = string.Empty;
    [ObservableProperty] private TechnicalAnalysisDto? selectedAnalysis;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool hasResults;
    [ObservableProperty] private bool hasSearched;

    public bool ShowHistoryGridEmpty => HasSearched && !HasResults;

    public bool ShowSelectionPlaceholder => SelectedAnalysis is null;

    partial void OnSelectedAnalysisChanged(TechnicalAnalysisDto? value)
    {
        OnPropertyChanged(nameof(ShowSelectionPlaceholder));
    }

    partial void OnHasSearchedChanged(bool value) => OnPropertyChanged(nameof(ShowHistoryGridEmpty));

    partial void OnHasResultsChanged(bool value) => OnPropertyChanged(nameof(ShowHistoryGridEmpty));

    [RelayCommand]
    private async Task SearchAsync()
    {
        var serial = SerialSearch.Trim();
        if (string.IsNullOrWhiteSpace(serial))
        {
            StatusMessage = "Informe um serial para busca.";
            return;
        }

        var result = await useCase.ExecuteAsync(serial);
        Analyses.Clear();
        foreach (var item in result.Value ?? [])
        {
            Analyses.Add(item);
        }

        HasResults = Analyses.Count > 0;
        HasSearched = true;
        StatusMessage = result.IsSuccess
            ? (HasResults ? $"{Analyses.Count} analise(s) encontradas." : "Nenhuma análise encontrada para este serial.")
            : result.Message;
    }

    public async Task SearchBySerialAsync(string serial)
    {
        SerialSearch = serial;
        await SearchAsync();
    }
}
