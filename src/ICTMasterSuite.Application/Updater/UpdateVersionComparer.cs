namespace ICTMasterSuite.Application.Updater;

/// <summary>Compara versões para decidir se há atualização disponível (mesma regra do updater).</summary>
public static class UpdateVersionComparer
{
    public static bool IsRemoteNewer(string current, string latest)
    {
        var a = NormalizeVersion(current);
        var b = NormalizeVersion(latest);
        if (Version.TryParse(a, out var v1) && Version.TryParse(b, out var v2))
        {
            return v2 > v1;
        }

        return !string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeVersion(string v)
    {
        return v.Trim().TrimStart('v', 'V');
    }
}
