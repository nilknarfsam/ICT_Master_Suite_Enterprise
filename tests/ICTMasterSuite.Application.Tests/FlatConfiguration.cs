using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ICTMasterSuite.Application.Tests;

/// <summary>Configuração mínima só com chaves planas (ex.: Updater:LatestVersion), para testes sem pacote InMemory.</summary>
internal sealed class FlatConfiguration : IConfiguration
{
    private readonly Dictionary<string, string?> _data;

    public FlatConfiguration(IReadOnlyDictionary<string, string?> data)
    {
        _data = new Dictionary<string, string?>(data, StringComparer.OrdinalIgnoreCase);
    }

    public string? this[string key]
    {
        get => _data.TryGetValue(key, out var v) ? v : null;
        set => _data[key] = value;
    }

    public IConfigurationSection GetSection(string key) => new FlatSection(this, key);

    public IEnumerable<IConfigurationSection> GetChildren() => [];

    public IChangeToken GetReloadToken() => new CancellationChangeToken(CancellationToken.None);
}

internal sealed class FlatSection : IConfigurationSection
{
    private readonly IConfiguration _root;
    private readonly string _path;

    public FlatSection(IConfiguration root, string path)
    {
        _root = root;
        _path = path;
    }

    public string Key => _path.Split(':').Last();

    public string Path => _path;

    public string? Value
    {
        get => _root[_path];
        set => _root[_path] = value;
    }

    public string? this[string childKey]
    {
        get => _root[$"{_path}:{childKey}"];
        set => _root[$"{_path}:{childKey}"] = value;
    }

    public IEnumerable<IConfigurationSection> GetChildren() => [];

    public IConfigurationSection GetSection(string key) => new FlatSection(_root, $"{_path}:{key}");

    public IChangeToken GetReloadToken() => new CancellationChangeToken(CancellationToken.None);
}
