using ICTMasterSuite.Application.Abstractions.Persistence;
using ICTMasterSuite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICTMasterSuite.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IctMasterSuiteDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> ListAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var usersQuery = dbContext.Users.Include(x => x.Role).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            usersQuery = usersQuery.Where(x =>
                x.FullName.Contains(query) ||
                x.Username.Contains(query) ||
                x.Email.Contains(query));
        }

        return await usersQuery.OrderBy(x => x.FullName).ToListAsync(cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }
}
