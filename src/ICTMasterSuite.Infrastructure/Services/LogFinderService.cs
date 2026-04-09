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

    public async Task<List<LogFile>> BuscarAsync(
        string termo,
        List<string> diretorios,
        CancellationToken cancellationToken = default)
    {
        var results = new List<LogFile>();
        var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var normalizedTerm = termo?.Trim() ?? string.Empty;

        foreach (var directory in diretorios.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<string> filePaths;
            try
            {
                filePaths = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (IOException)
            {
                continue;
            }

            try
            {
                foreach (var filePath in filePaths)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var fileName = Path.GetFileName(filePath);
                    var extension = Path.GetExtension(filePath).ToLowerInvariant();

                    if (!AllowedExtensions.Contains(extension))
                    {
                        continue;
                    }

                    if (fileName.Contains("pass", StringComparison.OrdinalIgnoreCase))
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

                    var lastWrite = File.GetLastWriteTime(filePath);
                    results.Add(new LogFile(fileName, fullPath, lastWrite));
                }
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (IOException)
            {
                continue;
            }
        }

        var ordered = results.OrderByDescending(x => x.Data).ToList();
        return await Task.FromResult(ordered);
    }

    public async Task<IReadOnlyCollection<LogFile>> SearchAsync(
        string term,
        IReadOnlyCollection<string> directories,
        CancellationToken cancellationToken = default)
    {
        var result = await BuscarAsync(term, directories.ToList(), cancellationToken);
        return result;
    }
}
