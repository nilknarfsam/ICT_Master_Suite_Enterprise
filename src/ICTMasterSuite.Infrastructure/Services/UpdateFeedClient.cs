using System.Net;
using ICTMasterSuite.Application.Abstractions.Updater;
using ICTMasterSuite.Application.Updater;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

/// <summary>Implementação HTTP/JSON do feed remoto; parsing delegado a <see cref="UpdateFeedJsonParser" />.</summary>
public sealed class UpdateFeedClient(HttpClient httpClient) : IUpdateFeedClient
{
    public async Task<RemoteUpdateFeedSnapshot?> FetchAsync(string feedUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync(feedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Warning("UpdateFeedClient: HTTP {Status} ao obter feed {Url}", response.StatusCode, feedUrl);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var snapshot = UpdateFeedJsonParser.TryParse(json);
            if (snapshot is null)
            {
                Log.Warning("UpdateFeedClient: JSON inválido ou sem latestVersion em {Url}", feedUrl);
            }

            return snapshot;
        }
        catch (OperationCanceledException)
        {
            Log.Warning("UpdateFeedClient: operação cancelada ao acessar {Url}", feedUrl);
            return null;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "UpdateFeedClient: falha ao consumir feed remoto (resiliente). Url={Url}", feedUrl);
            return null;
        }
    }
}
