using System.Windows;
using ICTMasterSuite.Presentation.Wpf.ViewModels;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.LoginSucceeded += OnLoginSucceeded;
        DataContext = viewModel;
    }

    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
