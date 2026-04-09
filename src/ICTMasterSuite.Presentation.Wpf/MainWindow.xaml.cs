using System.Windows;
using ICTMasterSuite.Infrastructure.Persistence;
using ICTMasterSuite.Presentation.Wpf.Services;
using ICTMasterSuite.Presentation.Wpf.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IApplicationOrchestrator _orchestrator;
    private readonly AppSessionState _appSessionState;
    private readonly AuthenticatedUserState _authenticatedUserState;
    private readonly IctMasterSuiteDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(
        MainWindowViewModel viewModel,
        IApplicationOrchestrator orchestrator,
        AppSessionState appSessionState,
        AuthenticatedUserState authenticatedUserState,
        IctMasterSuiteDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _orchestrator = orchestrator;
        _appSessionState = appSessionState;
        _authenticatedUserState = authenticatedUserState;
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _viewModel.LoggedOut += OnLoggedOut;
        _viewModel.LogSearch.RegisterAnalysisRequested += OnRegisterAnalysisRequested;
        _viewModel.LogSearch.ViewHistoryRequested += OnViewHistoryRequested;
        _viewModel.LogSearch.SearchKnowledgeRequested += OnSearchKnowledgeRequested;
        _viewModel.LoginRequested += OnLoginRequested;
        _viewModel.RefreshConnectivityRequested += OnRefreshConnectivityRequested;
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private async void OnLoggedOut(object? sender, EventArgs e)
    {
        await _orchestrator.LogoutAsync();
    }

    private async void OnLoginRequested(object? sender, EventArgs e)
    {
        if (await _orchestrator.ShowLoginAsync())
        {
            _ = _viewModel.InitializeAsync();
        }
    }

    private async void OnRefreshConnectivityRequested(object? sender, EventArgs e)
    {
        var online = await ProbeConnectivityAsync();
        _appSessionState.SetConnectivity(online ? ConnectivityState.Online : ConnectivityState.Offline);
        if (!online)
        {
            _authenticatedUserState.Clear();
            _appSessionState.SetAuthentication(AuthenticationState.Guest);
        }

        await _viewModel.InitializeAsync();
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
        var registerAnalysisWindow = _serviceProvider.GetRequiredService<RegisterAnalysisWindow>();
        registerAnalysisWindow.Owner = this;
        registerAnalysisWindow.Initialize(log);
        var success = registerAnalysisWindow.ShowDialog() is true;
        _viewModel.LogSearch.StatusMessage = success
            ? "Análise registrada e vinculada ao histórico técnico."
            : _viewModel.LogSearch.StatusMessage;
    }

    private async Task<bool> ProbeConnectivityAsync()
    {
        try
        {
            return await _dbContext.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    public Task ReinitializeAsync()
    {
        return _viewModel.InitializeAsync();
    }
}