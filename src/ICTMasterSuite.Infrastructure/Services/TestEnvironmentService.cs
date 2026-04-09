using Serilog;

namespace ICTMasterSuite.Infrastructure.Services;

public sealed class TestEnvironmentService
{
    private const string BasePath = @"C:\ICT_TEST\";
    private static readonly string[] FolderNames = ["TRI", "AGILENT", "BACKUP"];

    public void EnsureTestFolders()
    {
        try
        {
            Directory.CreateDirectory(BasePath);

            foreach (var folderName in FolderNames)
            {
                Directory.CreateDirectory(Path.Combine(BasePath, folderName));
            }

            EnsureSampleFile("TRI", "TEST123_fail.csv", "serial,result,error");
            EnsureSampleFile("AGILENT", "TEST456_log.dcl", "sample agilent log");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Falha ao preparar ambiente local de testes em {BasePath}.", BasePath);
        }
    }

    private static void EnsureSampleFile(string folderName, string fileName, string content)
    {
        var filePath = Path.Combine(BasePath, folderName, fileName);
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, content);
        }
    }
}
