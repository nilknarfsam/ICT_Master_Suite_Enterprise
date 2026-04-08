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

        StatusMessage = result.IsSuccess ? $"{Analyses.Count} analise(s) encontradas." : result.Message;
    }
}
