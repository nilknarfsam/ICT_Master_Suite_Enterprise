using ICTMasterSuite.Application.Authentication.Dtos;
using ICTMasterSuite.Application.Common;
using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hash);
}

public interface ISessionStore
{
    Task SetCurrentSessionAsync(UserSession session, CancellationToken cancellationToken = default);
    Task<UserSession?> GetCurrentSessionAsync(CancellationToken cancellationToken = default);
    Task ClearCurrentSessionAsync(CancellationToken cancellationToken = default);
}

public interface ICurrentUserContext
{
    Task<SignInResponse?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}

public interface IAuditLogger
{
    Task<Result> WriteAsync(string action, string details, CancellationToken cancellationToken = default);
}
