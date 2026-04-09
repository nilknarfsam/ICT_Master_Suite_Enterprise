using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class LogFile : EntityBase
{
    public string Nome { get; private set; }
    public string Caminho { get; private set; }
    public DateTime Data { get; private set; }

    public string FileName => Nome;
    public string FullPath => Caminho;
    public string Extension => Path.GetExtension(Caminho);
    public DateTime LastModifiedAt => Data;

    public LogFile(string nome, string caminho, DateTime data)
    {
        Nome = nome;
        Caminho = caminho;
        Data = data;
    }

    public LogFile(string fileName, string fullPath, string extension, DateTime lastModifiedAt)
        : this(fileName, fullPath, lastModifiedAt)
    {
    }
}
