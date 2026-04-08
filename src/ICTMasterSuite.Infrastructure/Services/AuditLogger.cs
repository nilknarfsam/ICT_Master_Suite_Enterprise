using ICTMasterSuite.Application.Abstractions.Security;
using ICTMasterSuite.Application.Common;
using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class AuditLogger : IAuditLogger
{
    public Task<Result> WriteAsync(string action, string details, CancellationToken cancellationToken = default)
    {
        Log.Information("AUDIT | {Action} | {Details}", action, details);
        return Task.FromResult(Result.Success());
    }
}
