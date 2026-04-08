using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ICTMasterSuite.Infrastructure.Persistence;

/// <summary>
/// Used by EF Core tools (dotnet ef migrations) without starting the WPF host or requiring a live corporate database.
/// Connection string is design-time only and is not used at application runtime.
/// </summary>
public sealed class IctMasterSuiteDbContextFactory : IDesignTimeDbContextFactory<IctMasterSuiteDbContext>
{
    public IctMasterSuiteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IctMasterSuiteDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=ICTMasterSuite_DesignOnly;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True");
        return new IctMasterSuiteDbContext(optionsBuilder.Options);
    }
}
