namespace Uppgift;

public static class Logger
{
    public static string LogPath { get; set; } = string.Empty;

    public static void Log(string textContent)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LogPath))
                return;

            File.AppendAllTextAsync(LogPath, textContent + Environment.NewLine);
        }
        catch
        {
            // Ignorera fel för att inte blockera tester
        }
    }
}