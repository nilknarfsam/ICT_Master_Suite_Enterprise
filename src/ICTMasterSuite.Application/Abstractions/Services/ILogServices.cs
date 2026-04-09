using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Application.Abstractions.Services;

public interface ILogFinderService
{
    Task<List<LogFile>> BuscarAsync(string termo, List<string> diretorios, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LogFile>> SearchAsync(string term, IReadOnlyCollection<string> directories, CancellationToken cancellationToken = default);
}

public interface ILogParserService
{
    Task<ParsedLog> ParseAsync(LogFile logFile, CancellationToken cancellationToken = default);
}
