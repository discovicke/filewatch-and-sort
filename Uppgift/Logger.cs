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

            var dir = Path.GetDirectoryName(LogPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.AppendAllText(LogPath, textContent + "/n");
        }
        catch
        {
            //Ignorerar för att hålla igång testerna
        }
    }
}