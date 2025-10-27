using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public async Task HandleFiles(string sourcePath, DirectorySetting setting)
    {
        string ext = Path.GetExtension(sourcePath).ToLowerInvariant();
        if (!setting.Types.Contains(ext))
            return;

        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(setting.Output, fileName);

        var maxAttempts = 10;
        var delayMs = 300;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    await Task.Delay(delayMs);
                    continue;
                }

                File.Move(sourcePath, destPath, true);
                Logger.Log($"{fileName} flyttades till {setting.Name}");
                return;
            }
            
            catch (FileNotFoundException)
            {
                return;
            }
            
            catch (IOException)
            {
                await Task.Delay(delayMs);
            }   
            
            catch (UnauthorizedAccessException)
            {
                await Task.Delay(delayMs);
            }
            
            Logger.Log($"Kunde inte flytta {fileName} efter {maxAttempts} försök.");
        }
    }
}