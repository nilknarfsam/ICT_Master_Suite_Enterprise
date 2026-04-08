using ICTMasterSuite.Application.Updater;

namespace ICTMasterSuite.Application.Abstractions.Updater;

/// <summary>Cliente explícito para obtenção e interpretação do feed remoto de versão (sem HTTP na camada de domínio).</summary>
public interface IUpdateFeedClient
{
    /// <summary>
    /// Obtém o feed em <paramref name="feedUrl" /> e retorna o snapshot interpretado.
    /// Retorna <c>null</c> se a requisição falhar, o payload for inválido ou <c>latestVersion</c> estiver ausente.
    /// </summary>
    Task<RemoteUpdateFeedSnapshot?> FetchAsync(string feedUrl, CancellationToken cancellationToken = default);
}
