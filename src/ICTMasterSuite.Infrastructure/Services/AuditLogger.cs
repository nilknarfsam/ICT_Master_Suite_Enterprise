using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Common;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class AuditLogger : IAuditLogger
{
    public Task<Result> WriteAsync(
        string action,
        string details,
        CancellationToken cancellationToken = default,
        string? module = null,
        string? user = null)
    {
        Log.Information("AUDIT | Module={Module} | User={User} | Action={Action} | Details={Details}",
            module ?? "N/A",
            user ?? "N/A",
            action,
            details);
        return Task.FromResult(Result.Success());
    }
}
