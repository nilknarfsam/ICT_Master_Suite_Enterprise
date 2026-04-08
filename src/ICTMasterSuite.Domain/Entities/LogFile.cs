using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class LogFile : EntityBase
{
    public string FileName { get; private set; }
    public string FullPath { get; private set; }
    public string Extension { get; private set; }
    public DateTime LastModifiedAt { get; private set; }

    public LogFile(string fileName, string fullPath, string extension, DateTime lastModifiedAt)
    {
        FileName = fileName;
        FullPath = fullPath;
        Extension = extension;
        LastModifiedAt = lastModifiedAt;
    }
}
