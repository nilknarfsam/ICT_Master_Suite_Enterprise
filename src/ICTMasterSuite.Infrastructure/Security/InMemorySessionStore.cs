using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Infrastructure.Security;

public sealed class InMemorySessionStore : ISessionStore
{
    private UserSession? _current;

    public Task SetCurrentSessionAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _current = session;
        return Task.CompletedTask;
    }

    public Task<UserSession?> GetCurrentSessionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_current);
    }

    public Task ClearCurrentSessionAsync(CancellationToken cancellationToken = default)
    {
        _current = null;
        return Task.CompletedTask;
    }
}
