using System.Windows;
using ICTMasterSuite.Infrastructure;
using ICTMasterSuite.Presentation.Wpf.Services;
using ICTMasterSuite.Presentation.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    private Microsoft.Extensions.Hosting.IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "Erro nao tratado na UI.");
            MessageBox.Show(BuildUnhandledErrorMessage(args.Exception), "ICT Master Suite", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            Log.Fatal(args.ExceptionObject as Exception, "Erro fatal nao tratado.");
        };

        try
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .UseSerilog((_, _, logger) => logger.AddCentralizedLogging())
                .ConfigureServices((context, services) =>
                {
                    ICTMasterSuite.Application.DependencyInjection.AddApplication(services);
                    services.AddInfrastructure(context.Configuration);
                    services.AddSingleton<ThemeService>();
                    services.AddSingleton<AuthenticatedUserState>();
                    services.AddSingleton<AppSessionState>();
                    services.AddSingleton<FinderResultsState>();
                    services.AddSingleton<IApplicationOrchestrator, ApplicationOrchestrator>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<UserManagementViewModel>();
                    services.AddTransient<LogSearchViewModel>();
                    services.AddTransient<TechnicalHistoryViewModel>();
                    services.AddTransient<KnowledgeBaseViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ReportsViewModel>();
                    services.AddTransient<SystemSettingsViewModel>();
                    services.AddTransient<UpdaterViewModel>();
                    services.AddTransient<RegisterAnalysisViewModel>();
                    services.AddTransient<RegisterAnalysisWindow>();
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();

            _host.Start();
            var orchestrator = _host.Services.GetRequiredService<IApplicationOrchestrator>();
            await orchestrator.StartAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Falha durante startup da aplicacao.");
            MessageBox.Show(BuildStartupErrorMessage(ex), "ICT Master Suite", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            _host.Services.GetRequiredService<IApplicationOrchestrator>().Shutdown();
        }

        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static string BuildUnhandledErrorMessage(Exception exception)
    {
        if (!IsDevelopmentEnvironment())
        {
            return "Ocorreu um erro inesperado. Consulte os logs.";
        }

        return $"Ocorreu um erro inesperado.\n{exception.GetType().Name}: {exception.Message}";
    }

    private static string BuildStartupErrorMessage(Exception exception)
    {
        if (!IsDevelopmentEnvironment())
        {
            return "Falha ao inicializar o aplicativo. Consulte os logs.";
        }

        return $"Falha ao inicializar o aplicativo.\n{exception.GetType().Name}: {exception.Message}";
    }

    private static bool IsDevelopmentEnvironment()
    {
        var environment =
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
    }
}
