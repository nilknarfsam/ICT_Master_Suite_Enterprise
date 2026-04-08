using System.Reflection;
using ICTMasterSuite.Application.Abstractions.Updater;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class ReflectionCurrentApplicationVersionProvider : ICurrentApplicationVersionProvider
{
    public string GetCurrentVersion()
    {
        try
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            if (ver is null)
            {
                return "1.0.0";
            }

            return $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }
        catch
        {
            return "1.0.0";
        }
    }
}
