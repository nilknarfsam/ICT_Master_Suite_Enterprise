namespace ICTMasterSuite.Presentation.Wpf.Services;

public interface IApplicationOrchestrator
{
    Task StartAsync();
    Task<bool> ShowLoginAsync();
    Task ShowMainAsync();
    Task LogoutAsync();
    void Shutdown();
}
