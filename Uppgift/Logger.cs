namespace Uppgift;

public static class Logger
{
    public static string LogPath { get; set; } = string.Empty;

    public static void Log(string message)
    {
        File.AppendAllText(LogPath, message + Environment.NewLine);
    }
}