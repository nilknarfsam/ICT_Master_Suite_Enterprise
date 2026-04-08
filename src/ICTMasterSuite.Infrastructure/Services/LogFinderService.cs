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
        var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var normalizedTerm = term?.Trim() ?? string.Empty;

        foreach (var directory in directories.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var filePath in EnumerateFilesSafe(directory, cancellationToken))
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

                var fullPath = Path.GetFullPath(filePath);
                if (!uniquePaths.Add(fullPath))
                {
                    continue;
                }

                var lastWrite = File.GetLastWriteTimeUtc(filePath);
                results.Add(new LogFile(fileName, fullPath, extension, lastWrite));
            }
        }

        return Task.FromResult<IReadOnlyCollection<LogFile>>(
            results.OrderByDescending(x => x.LastModifiedAt).ToList());
    }

    private static IEnumerable<string> EnumerateFilesSafe(string rootDirectory, CancellationToken cancellationToken)
    {
        var pending = new Stack<string>();
        pending.Push(rootDirectory);

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var current = pending.Pop();

            IEnumerable<string> files = [];
            IEnumerable<string> directories = [];

            try
            {
                files = Directory.EnumerateFiles(current);
                directories = Directory.EnumerateDirectories(current);
            }
            catch
            {
                continue;
            }

            foreach (var file in files)
            {
                yield return file;
            }

            foreach (var directory in directories)
            {
                pending.Push(directory);
            }
        }
    }
}
