using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Dashboard.UseCases;
using ICTMasterSuite.Application.Dashboard.Dtos;
using System.Collections.ObjectModel;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class DashboardViewModel(GetDashboardSummaryUseCase useCase) : ObservableObject
{
    [ObservableProperty] private int totalAnalyses;
    [ObservableProperty] private int activeArticles;
    [ObservableProperty] private int failCount;
    [ObservableProperty] private int passCount;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<SerialRecurrenceDto> TopSerials { get; } = [];

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var result = await useCase.ExecuteAsync();
        if (!result.IsSuccess || result.Value is null)
        {
            StatusMessage = result.Message;
            return;
        }

        TotalAnalyses = result.Value.TotalAnalyses;
        ActiveArticles = result.Value.ActiveKnowledgeArticles;
        FailCount = result.Value.FailCount;
        PassCount = result.Value.PassCount;
        TopSerials.Clear();
        foreach (var serial in result.Value.TopRecurringSerials) TopSerials.Add(serial);
        StatusMessage = "Dashboard atualizado.";
    }

    public Task RefreshDataAsync() => RefreshAsync();
}
