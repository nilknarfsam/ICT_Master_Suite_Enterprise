using System.Windows;
using ICTMasterSuite.Domain.Entities;
using ICTMasterSuite.Presentation.Wpf.ViewModels;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class RegisterAnalysisWindow : Window
{
    private readonly RegisterAnalysisViewModel _viewModel;

    public RegisterAnalysisWindow(RegisterAnalysisViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.RequestClose += (_, success) =>
        {
            DialogResult = success;
            Close();
        };
        DataContext = _viewModel;
    }

    public void Initialize(ParsedLog log)
    {
        _viewModel.InitializeFromLog(log);
    }
}
