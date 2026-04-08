using System.Windows;
using ICTMasterSuite.Presentation.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        _viewModel.LoggedOut += OnLoggedOut;
        _viewModel.LogSearch.RegisterAnalysisRequested += OnRegisterAnalysisRequested;
        _viewModel.LogSearch.ViewHistoryRequested += OnViewHistoryRequested;
        _viewModel.LogSearch.SearchKnowledgeRequested += OnSearchKnowledgeRequested;
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void OnLoggedOut(object? sender, EventArgs e)
    {
        Hide();
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        if (loginWindow.ShowDialog() is true)
        {
            Show();
            _ = _viewModel.InitializeAsync();
            return;
        }

        Close();
    }

    private async void OnViewHistoryRequested(object? sender, string serialNumber)
    {
        _viewModel.NavigateToModule(Domain.Enums.SystemModule.TechnicalHistory);
        await _viewModel.TechnicalHistory.SearchBySerialAsync(serialNumber);
    }

    private async void OnSearchKnowledgeRequested(object? sender, (string Model, string Term) query)
    {
        _viewModel.NavigateToModule(Domain.Enums.SystemModule.KnowledgeBase);
        await _viewModel.KnowledgeBase.SearchRelatedAsync(query.Model, query.Term);
        _viewModel.Reports.ApplyKnowledgeFilter(query.Model, query.Term);
    }

    private void OnRegisterAnalysisRequested(object? sender, Domain.Entities.ParsedLog log)
    {
        var window = _serviceProvider.GetRequiredService<RegisterAnalysisWindow>();
        window.Owner = this;
        window.Initialize(log);
        var success = window.ShowDialog() is true;
        _viewModel.LogSearch.StatusMessage = success
            ? "Análise registrada e vinculada ao histórico técnico."
            : _viewModel.LogSearch.StatusMessage;
    }
}