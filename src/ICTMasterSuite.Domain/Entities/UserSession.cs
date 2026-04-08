using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class UserSession : EntityBase
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string SessionToken { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public bool IsActive => EndedAt is null;

    public UserSession(Guid userId, string sessionToken)
    {
        UserId = userId;
        SessionToken = sessionToken;
        StartedAt = DateTime.UtcNow;
    }

    public void End()
    {
        EndedAt = DateTime.UtcNow;
        Touch();
    }
}
