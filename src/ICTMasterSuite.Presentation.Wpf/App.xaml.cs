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
                services.AddSingleton<FinderResultsState>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<LoginWindow>();
                services.AddTransient<UserManagementViewModel>();
                services.AddSingleton<LogSearchViewModel>();
                services.AddTransient<TechnicalHistoryViewModel>();
                services.AddTransient<KnowledgeBaseViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<ReportsViewModel>();
                services.AddTransient<SystemSettingsViewModel>();
                services.AddTransient<UpdaterViewModel>();
                services.AddTransient<RegisterAnalysisViewModel>();
                services.AddTransient<RegisterAnalysisWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();

        /*
         * Database startup policy (corporate):
         * - Primary path is always EF Core Migrate() so schema matches the migration history.
         * - The previous silent fallback Migrate -> EnsureCreated() was removed: it could mask failures and diverge schema from migrations.
         * - Optional EnsureCreated is allowed ONLY when Database:AllowEnsureCreatedFallback is true AND IHostEnvironment.IsDevelopment(),
         *   for local prototyping when migrations are not yet applied (explicit opt-in via appsettings.Development.json + DOTNET_ENVIRONMENT=Development).
         * - On failure without that escape hatch, we log, show a message, and shut down in a controlled way.
         */
        if (!TryInitializeDatabase(_host.Services))
        {
            Shutdown(1);
            base.OnStartup(e);
            return;
        }

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

    private static bool TryInitializeDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IctMasterSuiteDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var allowEnsureCreatedFallback = configuration.GetValue("Database:AllowEnsureCreatedFallback", false);

        try
        {
            dbContext.Database.Migrate();
            Log.Information("Banco de dados: Migrate() aplicado com sucesso.");
            return true;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Banco de dados: Migrate() falhou. Corrija conexao, permissoes e historico de migracoes.");

            var mayUseEnsureCreated = allowEnsureCreatedFallback && environment.IsDevelopment();
            if (!mayUseEnsureCreated)
            {
                MessageBox.Show(
                    "Nao foi possivel aplicar migracoes do banco de dados. Verifique a conexao e os logs.\n\n" +
                    "O fallback EnsureCreated esta desligado por padrao. Em desenvolvimento, use Database:AllowEnsureCreatedFallback=true em appsettings.Development.json " +
                    "e DOTNET_ENVIRONMENT=Development apenas como excecao explicita.",
                    "ICT Master Suite",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            Log.Warning(
                "Database:AllowEnsureCreatedFallback=true e ambiente Development: tentando EnsureCreated() como ultimo recurso local (nao usar em producao).");

            try
            {
                dbContext.Database.EnsureCreated();
                Log.Warning("EnsureCreated() concluido — o esquema pode nao refletir migracoes; use Migrate() para ambientes reais.");
                return true;
            }
            catch (Exception ex2)
            {
                Log.Fatal(ex2, "EnsureCreated() tambem falhou.");
                MessageBox.Show(
                    "Falha ao inicializar o banco de dados. Consulte os logs.",
                    "ICT Master Suite",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }
    }
}
