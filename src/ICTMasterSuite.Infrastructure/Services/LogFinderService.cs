using ICTMasterSuite.Application.Abstractions.Services;
using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class LogFinderService : ILogFinderService
{
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".csv",
        ".dcl",
        ".txt",
        ".log"
    ];

    public Task<IReadOnlyCollection<LogFile>> SearchAsync(
        string term,
        IReadOnlyCollection<string> directories,
        CancellationToken cancellationToken = default)
    {
        var results = new List<LogFile>();
        var normalizedTerm = term?.Trim() ?? string.Empty;

        foreach (var directory in directories.Where(Directory.Exists))
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var filePath in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(filePath);
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                {
                    continue;
                }

                if (fileName.Contains("pass", StringComparison.OrdinalIgnoreCase) ||
                    fileName.StartsWith("p_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(normalizedTerm) &&
                    !fileName.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var lastWrite = File.GetLastWriteTimeUtc(filePath);
                results.Add(new LogFile(fileName, filePath, extension, lastWrite));
            }
        }

        return Task.FromResult<IReadOnlyCollection<LogFile>>(
            results.OrderByDescending(x => x.LastModifiedAt).ToList());
    }
}
