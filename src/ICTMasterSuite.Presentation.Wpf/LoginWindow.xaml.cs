using System.Windows;
using System.Windows.Controls;
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

    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.Password = passwordBox.Password;
        }
    }
}
