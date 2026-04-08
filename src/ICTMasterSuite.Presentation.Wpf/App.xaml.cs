using System.Windows;
using ICTMasterSuite.Infrastructure;
using ICTMasterSuite.Infrastructure.Persistence;
using ICTMasterSuite.Presentation.Wpf.Services;
using ICTMasterSuite.Presentation.Wpf.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ICTMasterSuite.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "Erro nao tratado na UI.");
            MessageBox.Show("Ocorreu um erro inesperado. Consulte os logs.", "ICT Master Suite", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            Log.Fatal(args.ExceptionObject as Exception, "Erro fatal nao tratado.");
        };

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
                services.AddTransient<LoginViewModel>();
                services.AddTransient<LoginWindow>();
                services.AddTransient<UserManagementViewModel>();
                services.AddTransient<LogSearchViewModel>();
                services.AddTransient<TechnicalHistoryViewModel>();
                services.AddTransient<KnowledgeBaseViewModel>();
                services.AddTransient<RegisterAnalysisViewModel>();
                services.AddTransient<RegisterAnalysisWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();
        EnsureDatabaseInitialized(_host.Services);
        _host.Services.GetRequiredService<ThemeService>().ApplyTheme(AppTheme.Dark);

        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        var loginResult = loginWindow.ShowDialog();
        if (loginResult is true)
        {
            _host.Services.GetRequiredService<MainWindow>().Show();
        }
        else
        {
            Shutdown();
        }
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static void EnsureDatabaseInitialized(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IctMasterSuiteDbContext>();

        try
        {
            dbContext.Database.Migrate();
            Log.Information("Banco de dados atualizado com Migrate.");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Falha no Migrate. Tentando EnsureCreated.");
            dbContext.Database.EnsureCreated();
            Log.Information("Banco de dados garantido com EnsureCreated.");
        }
    }
}

