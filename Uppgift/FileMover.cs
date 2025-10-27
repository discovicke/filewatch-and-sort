using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public async Task HandleFiles(string fullPath, DirectorySetting setting)
    {
        string ext = Path.GetExtension(fullPath).ToLower();
        if (!setting.Types.Contains(ext))
            return;

        string fileName = Path.GetFileName(fullPath);
        string destPath = Path.Combine(setting.Output, fileName);

        int maxAttempts = 10;
        int delayMs = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    await Task.Delay(delayMs);
                    continue;
                }

                File.Move(fullPath, destPath, true);
                Logger.Log($"{fileName} flyttades till {setting.Name}");
                return;
            }
            catch (IOException)
            {
                await Task.Delay(delayMs);
            }
            
            Logger.Log($"Kunde inte flytta {fileName} efter {maxAttempts} försök.");
        }
    }
}