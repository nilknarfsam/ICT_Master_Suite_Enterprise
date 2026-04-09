using System.Windows;
using ICTMasterSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ICTMasterSuite.Presentation.Wpf.Services;

public sealed class ApplicationOrchestrator(
    IServiceProvider rootProvider,
    ThemeService themeService,
    AppSessionState sessionState) : IApplicationOrchestrator
{
    private IServiceScope? _mainScope;
    private MainWindow? _mainWindow;

    public async Task StartAsync()
    {
        var databaseReady = TryInitializeDatabase();
        sessionState.SetConnectivity(databaseReady ? ConnectivityState.Online : ConnectivityState.Offline);
        sessionState.SetAuthentication(AuthenticationState.Guest);

        themeService.ApplyTheme(AppTheme.Dark);

        if (databaseReady)
        {
            var loginSucceeded = await ShowLoginAsync();
            if (loginSucceeded)
            {
                sessionState.SetAuthentication(AuthenticationState.Authenticated);
            }
        }
        else
        {
            MessageBox.Show(
                "Inicializacao online indisponivel. O sistema sera aberto em modo visitante/offline com recursos limitados.",
                "ICT Master Suite",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        await ShowMainAsync();
    }

    public Task<bool> ShowLoginAsync()
    {
        using var loginScope = rootProvider.CreateScope();
        var loginWindow = loginScope.ServiceProvider.GetRequiredService<LoginWindow>();
        var result = loginWindow.ShowDialog() is true;
        return Task.FromResult(result);
    }

    public Task ShowMainAsync()
    {
        if (_mainWindow is not null)
        {
            _mainWindow.Show();
            return Task.CompletedTask;
        }

        _mainScope = rootProvider.CreateScope();
        _mainWindow = _mainScope.ServiceProvider.GetRequiredService<MainWindow>();
        _mainWindow.Closed += (_, _) =>
        {
            DisposeMainScope();
            // Guarantees app shutdown when the shell closes.
            System.Windows.Application.Current.Shutdown();
        };
        _mainWindow.Show();
        return Task.CompletedTask;
    }

    public async Task LogoutAsync()
    {
        if (_mainWindow is null)
        {
            return;
        }

        _mainWindow.Hide();
        var loginSucceeded = await ShowLoginAsync();
        if (!loginSucceeded)
        {
            _mainWindow.Close();
            return;
        }

        sessionState.SetAuthentication(AuthenticationState.Authenticated);
        _mainWindow.Show();
        await _mainWindow.ReinitializeAsync();
    }

    public void Shutdown()
    {
        DisposeMainScope();
    }

    private bool TryInitializeDatabase()
    {
        using var scope = rootProvider.CreateScope();
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
                Log.Warning("Sem fallback para EnsureCreated: inicializando shell em modo offline/restrito.");
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

    private void DisposeMainScope()
    {
        _mainWindow = null;
        _mainScope?.Dispose();
        _mainScope = null;
    }
}
