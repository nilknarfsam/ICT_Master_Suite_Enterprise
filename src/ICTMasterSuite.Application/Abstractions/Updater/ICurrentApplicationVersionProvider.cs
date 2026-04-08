namespace ICTMasterSuite.Application.Abstractions.Updater;

/// <summary>Fornece a versão da aplicação em execução (desacoplada para testes e serviços).</summary>
public interface ICurrentApplicationVersionProvider
{
    string GetCurrentVersion();
}
