using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Domain.Enums;
using ICTMasterSuite.Presentation.Wpf.Services;

namespace ICTMasterSuite.Presentation.Wpf.ViewModels;

public partial class MainWindowViewModel(
    AuthenticatedUserState authenticatedUserState,
    IAuthorizationService authorizationService,
    IAuthenticationService authenticationService,
    UserManagementViewModel userManagementViewModel,
    LogSearchViewModel logSearchViewModel) : ObservableObject
{
    public IReadOnlyCollection<ModuleNavigationItemViewModel> Modules { get; private set; } = [];
    public UserManagementViewModel UserManagement { get; } = userManagementViewModel;
    public LogSearchViewModel LogSearch { get; } = logSearchViewModel;

    [ObservableProperty]
    private ModuleNavigationItemViewModel? selectedModule;

    [ObservableProperty]
    private string loggedUserLabel = string.Empty;

    public event EventHandler? LoggedOut;

    public async Task InitializeAsync()
    {
        if (authenticatedUserState.Current is null)
        {
            return;
        }

        LoggedUserLabel = $"{authenticatedUserState.Current.FullName} ({authenticatedUserState.Current.RoleName})";
        Modules = await BuildModulesAsync(authenticatedUserState.Current.UserId);
        SelectedModule = Modules.FirstOrDefault();
        await UserManagement.InitializeAsync();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await authenticationService.SignOutAsync();
        authenticatedUserState.Clear();
        LoggedOut?.Invoke(this, EventArgs.Empty);
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
            new(SystemModule.Export, "Exportacao Excel/PDF", "Saidas corporativas para auditoria e reporting."),
            new(SystemModule.Audit, "Auditoria", "Rastreabilidade de acoes sensiveis do sistema."),
            new(SystemModule.OfflineSync, "Sync Offline (Futuro)", "Base preparada para sincronizacao em campo.")
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
}
