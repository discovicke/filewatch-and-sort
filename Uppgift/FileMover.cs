using Uppgift.Models;

namespace Uppgift;

public class FileMover
{
    public async Task HandleFiles(string sourcePath, DirectoryRule setting)
    {
        var ext = Path.GetExtension(sourcePath)
                            .ToLowerInvariant();
        if (!setting
                .Types
                .Contains(ext))
            return;

        var fileName = Path.GetFileName(sourcePath);
        var destPath = Path.Combine(setting.Output, fileName);

        const int maxAttempts = 10;
        const int delayMs = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    await Task.Delay(delayMs);
                    continue;
                }
                
                using (var stream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.None))
                { }

                File.Move(sourcePath, destPath, true);
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: {fileName} flyttades till {setting.Name}";

                Logger.Log(logMessage);
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
        }
    }
    
    public async Task ExistingFiles(DirectoryRule setting)
    {
        if (!Directory.Exists(setting.Input))
        {
            Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Input-mapp finns inte: {setting.Input}");
            return;
        }

        try
        {
            var files = Directory.GetFiles(setting.Input);

            foreach (var file in files)
            {
                await HandleFiles(file, setting);
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: Fel vid uppstartsbearbetning: {ex.Message}");
        }
    }
}