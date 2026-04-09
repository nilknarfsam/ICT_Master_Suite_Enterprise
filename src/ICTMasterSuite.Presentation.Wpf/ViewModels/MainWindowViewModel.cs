using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Application.Configuration;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class MainWindowViewModel(
    AuthenticatedUserState authenticatedUserState,
    AppSessionState appSessionState,
    IAuthorizationService authorizationService,
    IAuthenticationService authenticationService,
    UserManagementViewModel userManagementViewModel,
    LogSearchViewModel logSearchViewModel,
    TechnicalHistoryViewModel technicalHistoryViewModel,
    KnowledgeBaseViewModel knowledgeBaseViewModel,
    DashboardViewModel dashboardViewModel,
    ReportsViewModel reportsViewModel,
    SystemSettingsViewModel systemSettingsViewModel,
    UpdaterViewModel updaterViewModel) : ObservableObject
{
    public IReadOnlyCollection<ModuleNavigationItemViewModel> Modules { get; private set; } = [];
    public UserManagementViewModel UserManagement { get; } = userManagementViewModel;
    public LogSearchViewModel LogSearch { get; } = logSearchViewModel;
    public TechnicalHistoryViewModel TechnicalHistory { get; } = technicalHistoryViewModel;
    public KnowledgeBaseViewModel KnowledgeBase { get; } = knowledgeBaseViewModel;
    public DashboardViewModel Dashboard { get; } = dashboardViewModel;
    public ReportsViewModel Reports { get; } = reportsViewModel;
    public SystemSettingsViewModel Settings { get; } = systemSettingsViewModel;
    public UpdaterViewModel Updater { get; } = updaterViewModel;

    [ObservableProperty]
    private ModuleNavigationItemViewModel? selectedModule;

    [ObservableProperty]
    private string loggedUserLabel = string.Empty;

    [ObservableProperty]
    private string sessionStatusLabel = string.Empty;

    [ObservableProperty]
    private bool canAuthenticate = true;

    [ObservableProperty]
    private bool canUseRestrictedFinderActions;

    [ObservableProperty]
    private bool canShowLoginAction;

    [ObservableProperty]
    private bool canShowLogoutAction;

    [ObservableProperty]
    private bool isRestrictedMode;

    [ObservableProperty]
    private string restrictedModeMessage = string.Empty;

    public event EventHandler? LoggedOut;
    public event EventHandler? LoginRequested;
    public event EventHandler? RefreshConnectivityRequested;

    public async Task InitializeAsync()
    {
        CanAuthenticate = appSessionState.IsOnline;
        var isGuest = authenticatedUserState.Current is null || appSessionState.IsGuest;
        if (isGuest)
        {
            LoggedUserLabel = appSessionState.IsOnline
                ? "Modo visitante ativo (nao autenticado)."
                : "Modo visitante/offline ativo.";
            appSessionState.SetAuthentication(AuthenticationState.Guest);
            IsRestrictedMode = true;
            CanUseRestrictedFinderActions = false;
            CanShowLoginAction = true;
            CanShowLogoutAction = false;
            SessionStatusLabel = appSessionState.IsOnline ? "Conectado" : "Offline";
            RestrictedModeMessage = appSessionState.IsOnline
                ? "Recursos avancados exigem login. Finder e leitura de resultados permanecem liberados."
                : "Modo offline: recursos que dependem de autenticacao/banco estao indisponiveis.";
            Modules = BuildRestrictedModules();
            OnPropertyChanged(nameof(Modules));
            SelectedModule = Modules.FirstOrDefault();
            return;
        }

        var currentUser = authenticatedUserState.Current!;
        appSessionState.SetAuthentication(AuthenticationState.Authenticated);
        LoggedUserLabel = $"{currentUser.FullName} ({currentUser.RoleName})";
        SessionStatusLabel = appSessionState.IsOnline ? "Autenticado / Online" : "Autenticado / Offline";
        IsRestrictedMode = appSessionState.IsOffline;
        CanUseRestrictedFinderActions = appSessionState.IsOnline;
        CanShowLoginAction = false;
        CanShowLogoutAction = true;
        RestrictedModeMessage = appSessionState.IsOffline
            ? "Sessao autenticada, porem offline. Modulos que exigem banco/rede ficam bloqueados."
            : string.Empty;

        Modules = await BuildModulesAsync(currentUser.UserId);
        OnPropertyChanged(nameof(Modules));
        SelectedModule = Modules.FirstOrDefault();

        if (appSessionState.IsOffline)
        {
            return;
        }

        await UserManagement.InitializeAsync();
        await Dashboard.RefreshDataAsync();
        await Settings.LoadAsync();
        if (AutoUpdateCheckEligibility.ShouldRunAutomaticCheck(Settings.AutoCheckUpdates))
        {
            await Updater.CheckUpdatesAsync();
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (authenticatedUserState.Current is null)
        {
            return;
        }

        await authenticationService.SignOutAsync();
        authenticatedUserState.Clear();
        appSessionState.SetAuthentication(AuthenticationState.Guest);
        LoggedOut?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Login()
    {
        if (!CanAuthenticate)
        {
            RestrictedModeMessage = "Sem conectividade com banco/rede: login indisponivel no momento.";
            return;
        }

        LoginRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void RefreshConnectivity()
    {
        RefreshConnectivityRequested?.Invoke(this, EventArgs.Empty);
    }

    private async Task<IReadOnlyCollection<ModuleNavigationItemViewModel>> BuildModulesAsync(Guid userId)
    {
        var candidates = new List<ModuleNavigationItemViewModel>
        {
            new(SystemModule.Authentication, "Autenticacao e Acesso", "Login seguro, sessao e controle de permissao."),
            new(SystemModule.UsersAndProfiles, "Usuarios e Perfis", "Cadastro, papeis e governanca de usuarios."),
            new(SystemModule.LogFinder, "Finder de Logs", "Busca rapida e inteligente em bases de logs."),
            new(SystemModule.TriAndAgilentParser, "Parser TRI/Agilent", "Pipeline para interpretacao padronizada de logs."),
            new(SystemModule.TechnicalHistory, "Historico Tecnico", "Linha do tempo de eventos e manutencoes."),
            new(SystemModule.KnowledgeBase, "Base de Conhecimento", "Conteudo tecnico estruturado para consulta."),
            new(SystemModule.SystemSettings, "Configuracoes do Sistema", "Parametros globais e preferencia por ambiente."),
            new(SystemModule.Export, "Relatorios e Exportacao", "Saidas corporativas em Excel e PDF."),
            new(SystemModule.Audit, "Dashboard Executivo", "Indicadores tecnicos e monitoramento resumido."),
            new(SystemModule.OfflineSync, "Updater Enterprise", "Verificacao de versao e fluxo de atualizacao desacoplado.")
        };

        var filtered = new List<ModuleNavigationItemViewModel>();
        foreach (var module in candidates)
        {
            var hasPermission = await authorizationService.HasPermissionAsync(userId, module.Module, PermissionAction.View);
            if (hasPermission)
            {
                filtered.Add(module);
            }
        }

        return filtered;
    }

    private IReadOnlyCollection<ModuleNavigationItemViewModel> BuildRestrictedModules()
    {
        var modules = new List<ModuleNavigationItemViewModel>
        {
            new(SystemModule.Authentication, "Autenticacao e Acesso", "Entre com credenciais corporativas para liberar modulos avancados."),
            new(SystemModule.LogFinder, "Finder de Logs", "Busca e leitura local de logs em modo restrito.")
        };
        return modules;
    }

    public void NavigateToModule(SystemModule module)
    {
        SelectedModule = Modules.FirstOrDefault(x => x.Module == module) ?? SelectedModule;
    }
}
