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
}