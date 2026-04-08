using ICTMasterSuite.Domain.Entities;

namespace ICTMasterSuite.Presentation.Wpf.Services;

public sealed class FinderResultsState
{
    public IReadOnlyCollection<ParsedLog> LastResults { get; private set; } = Array.Empty<ParsedLog>();

    public void Set(IReadOnlyCollection<ParsedLog> results)
    {
        LastResults = results;
    }
}
